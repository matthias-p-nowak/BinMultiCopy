using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinMultiCopy {
    class MultiCopy {
        private List<string> sources;
        private List<string> targets;

        public MultiCopy(string[] args) {
            if (args.Length < 3) {
                Console.Error.WriteLine($"use {Environment.CommandLine} {{<source(file|dir)>}} -t {{<target(<dir>)}}");
                Environment.Exit(1);
            }
            Console.WriteLine($"multicopy for binary files, version {BuildVersion.Version}");
            sources = new List<string>();
            targets = new List<string>();
            var toTarget = false;
            foreach (string arg in args) {
                if (string.Equals(arg, "-t")) {
                    toTarget = true;
                }
                else {
                    if (toTarget)
                        targets.Add(arg);
                    else
                        sources.Add(arg);
                }
            }
        }

        static void Main(string[] args) {
            var mc = new MultiCopy(args);
            mc.Run();
            //Console.ReadLine();
        }

        private void Run() {
            var tmp = sources;
            sources = new List<string>();
            foreach (string path in tmp) {
                if (File.Exists(path)) {
                    sources.Add(path);
                    Console.WriteLine($"copying specified file {path}");
                }
                else if (Directory.Exists(path)) {
                    Console.WriteLine($"looking for files in {path}");
                    foreach (var extension in new string[] { "exe", "dll", "json", "config" }) {
                        var files = Directory.GetFiles(path, $"*.{extension}");
                        foreach (string fileName in files) {
                            sources.Add(fileName);
                            Console.WriteLine($"copying found {extension}-file '{fileName}'");
                        }
                    }
                }
                else {
                    Console.Error.WriteLine($"can't find {path}");
                    Environment.Exit(1);
                }
            }
            tmp = targets;
            targets = new List<string>();
            foreach (string path in tmp) {
                if (File.Exists(path)) {
                    Console.Error.WriteLine($"{path} should be a directory");
                    Environment.Exit(1);
                }
                else if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                if (Directory.Exists(path)) {
                    targets.Add(path);
                    Console.WriteLine($"copying to {path}");
                }
                else {
                    Console.Error.WriteLine($"target directory {path} does not exist");
                    Environment.Exit(1);
                }
            }
            Console.WriteLine("----- copying -----");
            foreach (string src in sources) {
                Console.WriteLine($"copying {src}");
                var basename = Path.GetFileName(src);
                var foe = Path.GetFileNameWithoutExtension(src);
                foreach (string dest in targets) {
                    var destFile = Path.Combine(dest, basename);
                    for (int attempt = 0; attempt < 5; ++attempt) {
                        try {
                            try {
                                File.Copy(src, destFile, true);
                                break;
                            }
                            catch (Exception) {
                            }
                            // only if exception
                            string bakFile = Path.Combine(dest, $"{foe}.x{attempt:d3}");
                            string tFile = Path.Combine(dest, $"{foe}.tmp");
                            File.Copy(src, tFile,true);
                            File.Replace(tFile, destFile, bakFile);
                            break;
                        }
                        catch (Exception ex) {
                            Console.WriteLine($"got exception {ex.GetType()} while replacing '{destFile}': {ex.Message}");
                            //try {
                            //    Console.WriteLine($"deleting {destFile}");
                            //    File.Delete(destFile);
                            //}
                            //catch (Exception) {
                            //    string tempDest = string.Empty;
                            //    for (int trial = 0; trial < 100; ++trial) {
                            //        try {
                            //            tempDest = foe + $".{trial:d3}";
                            //            File.Move(destFile, Path.Combine(dest, tempDest));
                            //            break;
                            //        }
                            //        catch (Exception mfex) {
                            //            Console.WriteLine($"failed to move {destFile} to {tempDest} {mfex.GetType()}: {mfex.Message}");
                            //            continue;
                            //        }
                            //    }
                            //}
                        }
                    }
                    Console.WriteLine($" -> {destFile}");
                    var files = Directory.GetFiles(dest, foe + ".0*");
                    foreach (var fn in files) {
                        try {
                            File.Delete(fn);
                        }
                        catch (Exception ex) {
                            Console.WriteLine($"  can't remove {fn} due to {ex.GetType()}: {ex.Message}");
                        }
                    }
                    files = Directory.GetFiles(dest, foe + ".x0*");
                    foreach (var fn in files) {
                        try {
                            File.Delete(fn);
                        }
                        catch (Exception ex) {
                            Console.WriteLine($"  can't remove {fn} due to {ex.GetType()}: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}
