using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text.RegularExpressions;

namespace aoc
{
    class Program
    {
        static void Day1()
        {
            var numbers = File.ReadAllLines("input.txt").Select(int.Parse).ToArray();
            for (int i = 0; i < numbers.Length; i++)
            for (int j = i + 1; j < numbers.Length; j++)
            for (int k = j + 1; k < numbers.Length; k++)
                if (numbers[i] + numbers[j] + numbers[k] == 2020)
                    Console.WriteLine(numbers[i] * numbers[j] * numbers[k]);
        }

        static void Day2()
        {
            var policies = File.ReadAllLines("input_2.txt");
            var valid = 0;
            foreach (var policy in policies)
            {
                var s = policy.Split(":");
                var pass = s[1].Trim();
                var p = s[0].Trim().Split(" ");
                var chr = p[1].Single();
                var bounds = p[0].Split("-");
                var lowerBound = int.Parse(bounds[0]);
                var upperBound = int.Parse(bounds[1]);

                // var chrCount = 0;
                // for (var i = 0; i < pass.Length; i++)
                //     if (pass[i] == chr)
                //         chrCount++;
                // if (chrCount >= lowerBound && chrCount <= upperBound)
                //     valid++;

                if (pass[lowerBound - 1] == chr && pass[upperBound - 1] != chr ||
                    pass[lowerBound - 1] != chr && pass[upperBound - 1] == chr)
                    valid++;
            }

            Console.WriteLine(valid);
        }

        static void Day3()
        {
            var map = File.ReadAllLines("input_3.txt").Select(x => x.Select(y => y == '#').ToArray()).ToArray();
            var len = map[0].Length;
            var counts = new[] {0, 0, 0, 0};
            var js = new[] {0, 0, 0, 0};
            for (var i = 0; i < map.Length; i++)
            {
                if (map[i][js[0] % len])
                    counts[0]++;
                if (map[i][js[1] % len])
                    counts[1]++;
                if (map[i][js[2] % len])
                    counts[2]++;
                if (map[i][js[3] % len])
                    counts[3]++;
                js[0] += 1;
                js[1] += 3;
                js[2] += 5;
                js[3] += 7;
            }

            var countY = 0;
            var jY = 0;
            for (var i = 0; i < map.Length; i += 2)
            {
                if (map[i][jY % len])
                    countY++;
                jY++;
            }

            Console.WriteLine(counts[0] * counts[1] * counts[2] * counts[3] * countY);
        }

        static void Day4()
        {
            var passports = File.ReadAllText("input_4.txt").Split("\n\n");
            var valid = 0;
            var required = new[] {"byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"};
            var eyes = new[] {"amb", "blu", "brn", "gry", "grn", "hzl", "oth"};
            var color = new Regex("^#([a-f0-9]{6})$", RegexOptions.Compiled);
            foreach (var passport in passports)
            {
                var passportData = passport.Split().ToDictionary(x => x.Split(":")[0], x => x.Split(":")[1]);
                if (required.All(x => passportData.ContainsKey(x)))
                {
                    var byr = int.Parse(passportData["byr"]);
                    var iyr = int.Parse(passportData["iyr"]);
                    var eyr = int.Parse(passportData["eyr"]);
                    if (!passportData["hgt"].EndsWith("cm") && !passportData["hgt"].EndsWith("in"))
                        continue;
                    var hgt = int.Parse(passportData["hgt"].Substring(0, passportData["hgt"].Length - 2));
                    var hgtVal = passportData["hgt"].Substring(passportData["hgt"].Length - 2);
                    var heigthValid = hgtVal == "cm" && hgt >= 150 && hgt <= 193 ||
                                      hgtVal == "in" && hgt >= 59 && hgt <= 76;
                    if (byr >= 1920 && byr <= 2002 && iyr >= 2010 && iyr <= 2020 && eyr >= 2020 && eyr <= 2030 &&
                        heigthValid && eyes.Contains(passportData["ecl"]) && passportData["pid"].Length == 9 &&
                        int.TryParse(passportData["pid"], out _) && color.IsMatch(passportData["hcl"]))
                        valid++;
                }
            }

            Console.WriteLine(valid);
        }

        static int BinSearch(string s, int start, int end)
        {
            // Console.WriteLine($"{s}, {start}, {end}");
            if (s.Length == 0)
                return start;
            var m = (int) Math.Floor((start + end) / 2.0);
            if (s[0] == 'F' || s[0] == 'L')
                return BinSearch(s.Substring(1), start, m);
            return BinSearch(s.Substring(1), m + 1, end);
        }

        static void Day5()
        {
            var seats = File.ReadAllLines("input_5.txt");
            var ids = new HashSet<int>();
            foreach (var seat in seats)
            {
                var row = BinSearch(seat.Substring(0, 7), 0, 127);
                var col = BinSearch(seat.Substring(7), 0, 7);
                ids.Add(row * 8 + col);
            }

            for (int i = 0; i < ids.Max(); i++)
            {
                if (!ids.Contains(i))
                    Console.WriteLine(i);
            }
        }

        static void Day6()
        {
            var answers = File.ReadAllText("input_6.txt").Split("\n\n");
            var regex = new Regex(@"\s");
            var count = 0;
            foreach (var answer in answers)
            {
                var distinct = regex.Replace(answer, "").ToHashSet();
                foreach (var ans in answer.Split().Select(x => x.ToHashSet()))
                {
                    distinct.IntersectWith(ans);
                }

                count += distinct.Count;
            }

            Console.WriteLine(count);
        }

        static void Dfs(Dictionary<string, (string, int)> parents, HashSet<(string, int)> visited,
            Dictionary<string, (string, int)[]> dict, (string, int) start)
        {
            visited.Add(start);
            if (!dict.ContainsKey(start.Item1))
                return;
            foreach (var node in dict[start.Item1])
                if (!visited.Contains(node))
                {
                    parents[node.Item1] = start;
                    Dfs(parents, visited, dict, node);
                }
        }

        static int Dfs2(HashSet<(string, int)> visited,
            Dictionary<string, (string, int)[]> dict, (string, int) start)
        {
            if (!dict.ContainsKey(start.Item1) || dict[start.Item1].Length == 0)
                return 0;
            var count = 0;
            foreach (var node in dict[start.Item1])
                count += node.Item2 + node.Item2 * Dfs2(visited, dict, node);

            return count;
        }

        static void Day7()
        {
            var rules = File.ReadAllLines("input_7.txt");
            var dict = new Dictionary<string, (string, int)[]>();
            foreach (var rule in rules)
            {
                var splitted = rule.Split(" bags contain ");
                var bag = splitted[0];
                if (splitted[1].Contains("no other bags"))
                {
                    dict[bag] = new (string, int)[0];
                    continue;
                }

                var children = splitted[1].Replace(" bags", "").Replace(" bag", "").Replace(".", "").Split(',')
                    .Select(x => x.Trim())
                    .Select(x => (x.Substring(x.IndexOf(' ') + 1),
                        int.Parse(x.Substring(0, x.IndexOf(' '))))).ToArray();
                // foreach (var child in children)
                // {
                //     if (dict.ContainsKey(child))
                //         dict[child].Add(bag);
                //     else
                //         dict[child] = new List<string> {bag};
                // }
                dict[bag] = children;
            }

            var start = "shiny gold";
            var visited = new HashSet<(string, int)>();
            var parents = new Dictionary<string, (string, int)>();
            // Dfs(parents, visited, dict, (start, 1));
            Console.WriteLine(Dfs2(visited, dict, (start, 1)));
        }

        static (bool Terminates, int Result) RunProgram((string Op, int N)[] instructions)
        {
            var visited = new HashSet<int>();
            var result = 0;
            for (var i = 0; i < instructions.Length; i++)
            {
                if (visited.Contains(i))
                    return (false, result);
                visited.Add(i);
                switch (instructions[i].Op)
                {
                    case "nop":
                        break;
                    case "acc":
                        result += instructions[i].N;
                        break;
                    case "jmp":
                        i += (instructions[i].N - 1);
                        break;
                }
            }

            return (true, result);
        }

        static void Day8()
        {
            var instructions = File.ReadAllLines("input_8.txt");
            for (var i = 0; i < instructions.Length; i++)
            {
                var parsed = instructions.Select(x => (Op: x.Split(' ')[0], N: int.Parse(x.Split(' ')[1]))).ToArray();
                if (parsed[i].Op == "acc")
                    continue;
                parsed[i].Op = parsed[i].Op == "nop" ? "jmp" : "nop";
                var (terminates, result) = RunProgram(parsed);
                if (terminates)
                    Console.WriteLine(result);
            }
        }

        static bool Valid(long[] numbers, int index)
        {
            for (var i = index - 25; i < index; i++)
            for (var j = i + 1; j < index; j++)
                if (numbers[i] + numbers[j] == numbers[index])
                    return true;

            return false;
        }

        static long Day9_1()
        {
            var numbers = File.ReadAllLines("input_9.txt").Select(long.Parse).ToArray();
            for (var i = 25; i < numbers.Length; i++)
            {
                if (!Valid(numbers, i))
                    return numbers[i];
            }

            return -1;
        }

        static void Day9_2(long invalid)
        {
            var numbers = File.ReadAllLines("input_9.txt").Select(long.Parse).ToArray();
            for (var i = 0; i < numbers.Length; i++)
            for (var j = i + 1; j < numbers.Length; j++)
                if (numbers.Skip(i).Take(j - i).Sum() == invalid)
                {
                    Console.WriteLine(i);
                    Console.WriteLine(j);
                    Console.WriteLine(numbers[i] + numbers[j - 1]);
                }
        }

        static BigInteger CountArrangemets(Dictionary<int, BigInteger> cache, int[] arr, int i)
        {
            if (i == arr.Length - 1)
                return 1;
            BigInteger counts = 0;
            for (var j = i + 1; j < arr.Length; j++)
            {
                if (arr[j] - arr[i] <= 3 && cache.ContainsKey(j))
                {
                    counts += cache[j];
                }
                else if (arr[j] - arr[i] <= 3)
                {
                    cache[j] = CountArrangemets(cache, arr, j);
                    counts += cache[j];
                }
                else
                {
                    break;
                }
            }

            return counts;
        }

        static void Day10()
        {
            var jolts = File.ReadAllLines("input_10.txt").Select(int.Parse).OrderBy(x => x).ToArray();
            var diff1 = 0;
            var diff3 = 1;
            for (var i = 0; i < jolts.Length; i++)
            {
                var diff = 0;
                if (i == 0)
                    diff = jolts[0];
                else
                    diff = jolts[i] - jolts[i - 1];
                if (diff == 1)
                    diff1++;
                if (diff == 3)
                    diff3++;
            }

            // Console.WriteLine(diff1 * diff3);
            Console.WriteLine(CountArrangemets(new Dictionary<int, BigInteger>(), jolts, 0));
        }

        static void Main(string[] args)
        {
            Day10();
        }
    }
}