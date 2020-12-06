using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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

        static void Main(string[] args)
        {
            Day6();
        }
    }
}