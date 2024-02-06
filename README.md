# BinMultiCopy
Copies common used program files which might be in use.

## Background

Files that are in use like DLL's and programs can't be overwritten, since the running program has a lock on those files.

**Solution:** Locked files are locked for writing and therefore cannot be changed. However, it is possible to change the filename by setting the extension to *000* or similar. The normal rename command is also prevented by the lock. Hence, the filename must be *replaced*. This is a change in the directory, not on the file.

## Usage

It is simple, just use the command line with:
~~~SHELL
BinMultiCopy.exe {<source(file|dir)>} -t {<target(dir)>}
~~~
The first term is the multi copy executable. 

The next arguments can either denote files or directories to be copied. From directories only files ending in *exe*, *dll*, *json*, or *config* are copied.

The next argument is a simple ```-t``` that separates the source directories/files from the target.

The final arguments are a list of directories to copy the discovered files to.

## inner workings
The program first tries to copy the file into the target directory. If that fails, it copies the file into the directory with extension ".tmp". Then it does a file replacement, renaming the locked target file to ".000" and simultaneously the ".tmp" file into the target file. At most 5 attempts are made.