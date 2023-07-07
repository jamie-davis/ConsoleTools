using System.Collections.Generic;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

// ReSharper disable UnusedMember.Local

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests
{
    [UseReporter(typeof (CustomReporter))]
    public class Config3AcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleOutInterface;
        private ConsoleAdapter _console;
        private static readonly string _applicationName = "AcceptanceTest";

        [Command]
        [Description("Add file contents to the index.")]
        class AddCommand
        {
            [Positional("filepattern")]
            [Description("Files to add content from. Fileglobs (e.h. *.c) can be given to add all matching files. Also, a leading directory name (e.g. dir to add dir/file1 and dir/file2) can be given to all all files in the directory recursively.")]
            public string FilePattern { get; set; }

            [Option("dryrun", "n")]
            [Description("Don't actually add the file(s), just show if they exist and/or will be ignored.")]
            public bool DryRun { get; set; }

            [Option("verbose", "v")]
            [Description("Be verbose.")]
            public bool Verbose { get; set; }

            [Option("force", "f")]
            [Description("Allow adding otherwise ignored files.")]
            public bool Force { get; set; }

            [Option("interactive", "i")]
            [Description(@"Add modified contents in the working tree interactively to the index. Optional path arguments may be supplied to limit operation to a subset of the working tree. See ""Interactive mode"" for details.")]
            public bool Interactive { get; set; }

            [Option("patch", "p")]
            [Description(@"Interactively choose hunks of patch between the index and the work tree and add them to the index. This gives the user a chance to review the difference before adding modified contents to the index.

This effectively runs add --interactive, but bypasses the initial command menu and directly jumps to the patch subcommand. See ""Interactive mode"" for details.")]
            public bool Patch { get; set; }

            [Option("edit", "e")]
            [Description(@"Open the diff vs. the index in an editor and let the user edit it. After the editor was closed, adjust the hunk headers and apply the patch to the index.

The intent of this option is to pick and choose lines of the patch to apply, or even to modify the contents of lines to be staged. This can be quicker and more flexible than using the interactive hunk selector. However, it is easy to confuse oneself and create a patch that does not apply to the index.")]
            public bool Edit { get; set; }

            [Option("update", "u")]
            [Description(@"Only match <filepattern> against already tracked files in the index rather than the working tree. That means that it will never stage new files, but that it will stage modified new contents of tracked files and that it will remove files from the index if the corresponding files in the working tree have been removed.

If no <filepattern> is given, default to "".""; in other words, update all tracked files in the current directory and its subdirectories.")]
            public bool Update { get; set; }

            [Option("all", "A")]
            [Description(@"Like -u, but match <filepattern> against files in the working tree in addition to the index. That means that it will find new files as well as staging modified content and removing files that are no longer in the working tree.")]
            public bool All { get; set; }

            [Option("intent-to-add", "N")]
            [Description(@"Record only the fact that the path will be added later. An entry for the path is placed in the index with no content. This is useful for, among other things, showing the unstaged content of such files with git diff and committing them with git commit -a.")]
            public bool IntentToAdd { get; set; }

            [Option]
            [Description(@"Don't add the file(s), but only refresh their stat() information in the index.")]
            public bool Refresh { get; set; }

            [Option("ignore-errors")]
            [Description(@"If some files could not be added because of errors indexing them, do not abort the operation, but continue adding the others. The command shall still exit with non-zero status. The configuration variable add.ignoreErrors can be set to true to make this the default behaviour.")]
            public bool IgnoreErrors { get; set; }

            [Option("ignore-missing")]
            [Description(@"This option can only be used together with --dry-run. By using this option the user can check if any of the given files would be ignored, no matter if they are already present in the work tree or not.")]
            public bool IgnoreMissing { get; set; }
        }

        [Command]
        [Description("Clone a repository into a new directory")]
        class CloneCommand
        {
            [Positional("repository")]
            [Description("The (possibly remote) repository to clone from. See the URLS section below for more information on specifying repositories.")]
            public string Repository { get; set; }

            [Positional("directory", DefaultValue = null)]
            [Description(@"The name of a new directory to clone into. The ""humanish"" part of the source repository is used if no directory is explicitly given (repo for /path/to/repo.git and foo for host.xz:foo/.git). Cloning into an existing directory is only allowed if the directory is empty.")]
            public string Directory { get; set; }
 
            [Option("local", "l")]
            [Description(@"When the repository to clone from is on a local machine, this flag bypasses the normal ""git aware"" transport mechanism and clones the repository by making a copy of HEAD and everything under objects and refs directories. The files under .git/objects/ directory are hardlinked to save space when possible.

If the repository is specified as a local path (e.g., /path/to/repo), this is the default, and --local is essentially a no-op. If the repository is specified as a URL, then this flag is ignored (and we never use the local optimizations). Specifying --no-local will override the default when /path/to/repo is given, using the regular git transport instead.

To force copying instead of hardlinking (which may be desirable if you are trying to make a back-up of your repository), but still avoid the usual ""git aware"" transport mechanism, --no-hardlinks can be used.")]
            public bool Local { get; set; }

            [Option("no-hard-links")]
            [Description("Optimize the cloning process from a repository on a local filesystem by copying files under .git/objects directory.")]
            public bool NoHardLinks { get; set; }

            [Option("shared", "s")]
            [Description(@"When the repository to clone is on the local machine, instead of using hard links, automatically setup .git/objects/info/alternates to share the objects with the source repository. The resulting repository starts out without any object of its own.

NOTE: this is a possibly dangerous operation; do not use it unless you understand what it does. If you clone your repository using this option and then delete branches (or use any other git command that makes any existing commit unreferenced) in the source repository, some objects may become unreferenced (or dangling). These objects may be removed by normal git operations (such as git commit) which automatically call git gc --auto. (See git-gc(1).) If these objects are removed and were referenced by the cloned repository, then the cloned repository will become corrupt.

Note that running git repack without the -l option in a repository cloned with -s will copy objects from the source repository into a pack in the cloned repository, removing the disk space savings of clone -s. It is safe, however, to run git gc, which uses the -l option by default.

If you want to break the dependency of a repository cloned with -s on its source repository, you can simply run git repack -a to copy all objects from the source repository into a pack in the cloned repository.")]
            public bool Shared { get; set; }
            
            [Option("reference")]
            [Description(@"If the reference repository is on the local machine, automatically setup .git/objects/info/alternates to obtain objects from the reference repository. Using an already existing repository as an alternate will require fewer objects to be copied from the repository being cloned, reducing network and local storage costs.

NOTE: see the NOTE for the --shared option.")]
            public string ReferenceRepository { get; set; }

            [Option("quiet", "q")]
            [Description("Operate quietly. Progress is not reported to the standard error stream. This flag is also passed to the 'rsync' command when given.")]
            public bool Quiet { get; set; }

            [Option("verbose", "v")]
            [Description("Be verbose.")]
            public bool Verbose { get; set; }

            [Option("progress")]
            [Description("Progress status is reported on the standard error stream by default when it is attached to a terminal, unless -q is specified. This flag forces progress status even if the standard error stream is not directed to a terminal.")]
            public bool Progress { get; set; }

            [Option("no-checkout", "n")]
            [Description("No checkout of HEAD is performed after the clone is complete.")]
            public bool NoCheckout { get; set; }

            [Option("bare")]
            [Description("Make a bare GIT repository. That is, instead of creating <directory> and placing the administrative files in <directory>/.git, make the <directory> itself the $GIT_DIR. This obviously implies the -n because there is nowhere to check out the working tree. Also the branch heads at the remote are copied directly to corresponding local branch heads, without mapping them to refs/remotes/origin/. When this option is used, neither remote-tracking branches nor the related configuration variables are created.")]
            public bool Bare { get; set; }

            [Option("mirror")]
            [Description("Set up a mirror of the source repository. This implies --bare. Compared to --bare, --mirror not only maps local branches of the source to local branches of the target, it maps all refs (including remote-tracking branches, notes etc.) and sets up a refspec configuration such that all these refs are overwritten by a git remote update in the target repository.")]
            public bool Mirror { get; set; }

            [Option("origin", "o")]
            [Description("Instead of using the remote name origin to keep track of the upstream repository, use <name>.")]
            public string Origin { get; set; }

            [Option("branch", "b")]
            [Description(@"Instead of pointing the newly created HEAD to the branch pointed to by the cloned repository's HEAD, point to <name> branch instead. In a non-bare repository, this is the branch that will be checked out. --branch can also take tags and detaches the HEAD at that commit in the resulting repository.")]
            public string Branch { get; set; }

            [Option("upload-pack", "u")]
            [Description(@"When given, and the repository to clone from is accessed via ssh, this specifies a non-default path for the command run on the other end.")]
            public string UploadPack { get; set; }

            [Option("template")]
            [Description(@"Specify the directory from which templates will be used; (See the ""TEMPLATE DIRECTORY"" section of git-init(1).")]
            public string Template { get; set; }

            [Option("config", "c")]
            [Description(@"Set a configuration variable in the newly-created repository; this takes effect immediately after the repository is initialized, but before the remote history is fetched or any files checked out. The key is in the same format as expected by git-config(1) (e.g., core.eol=true). If multiple values are given for the same key, each value will be written to the config file. This makes it safe, for example, to add additional fetch refspecs to the origin remote.")]
            public List<string> Config  { get; set; }

            [Option("depth")]
            [Description(@"Create a shallow clone with a history truncated to the specified number of revisions. A shallow repository has a number of limitations (you cannot clone or fetch from it, nor push from nor into it), but is adequate if you are only interested in the recent history of a large project with a long history, and would want to send in fixes as patches.")]
            public int Depth  { get; set; }

            [Option("single-branch")]
            [Description(@"Clone only the history leading to the tip of a single branch, either specified by the --branch option or the primary branch remote's HEAD points at. When creating a shallow clone with the --depth option, this is the default, unless --no-single-branch is given to fetch the histories near the tips of all branches. Further fetches into the resulting repository will only update the remote-tracking branch for the branch this option was used for the initial cloning. If the HEAD at the remote did not point at any branch when --single-branch clone was made, no remote-tracking branch is created.")]
            public bool SingleBranch  { get; set; }

            [Option("recursive", "recursive-submodules")]
            [Description(@"After the clone is created, initialize all submodules within, using their default settings. This is equivalent to running git submodule update --init --recursive immediately after the clone is finished. This option is ignored if the cloned repository does not have a worktree/checkout (i.e. if any of --no-checkout/-n, --bare, or --mirror is given)")]
            public bool RecurseSubmodules  { get; set; }

            [Option("seperate-git-dir")]
            [Description(@"Instead of placing the cloned repository where it is supposed to be, place the cloned repository at the specified directory, then make a filesytem-agnostic git symbolic link to there. The result is git repository can be separated from working tree.")]
            public string SeperateGitDir  { get; set; }
        }
        public Config3AcceptanceTests()
        {
            _posix = new CommandLineInterpreterConfiguration(CommandLineParserConventions.PosixConventions);
            _msDos = new CommandLineInterpreterConfiguration(CommandLineParserConventions.MsDosConventions);
            _msStd = new CommandLineInterpreterConfiguration(CommandLineParserConventions.MicrosoftStandard);
            Configure(_posix);
            Configure(_msDos);
            Configure(_msStd);

            _consoleOutInterface = new ConsoleInterfaceForTesting();
            _consoleOutInterface.WindowWidth = 80;
            _consoleOutInterface.BufferWidth = 80;
            _console = new ConsoleAdapter(_consoleOutInterface);
        }

        private void Configure(CommandLineInterpreterConfiguration config)
        {
            config.Load(typeof(AddCommand));
            config.Load(typeof(CloneCommand));
        }

        [Fact]
        public void ConfigurationShouldBeDescribed()
        {
            CommandConfigDescriber.Describe(_posix, _console, "POSIX", CommandLineParserConventions.PosixConventions, CommandExecutionMode.CommandLine);
            var description = _consoleOutInterface.GetBuffer();
            Approvals.Verify(description);
        }

        [Fact]
        public void PosixStyle()
        {
            var commands = new[]
            {
                @"add *.c",
                @"clone --reference my2.6 git://git.kernel.org/pub/scm/.../linux-2.7 my2.7",
                @"clone git@github.com:whatever",
                @"clone git@github.com:whatever folder-name",
                @"clone --mirror https://github.com/exampleuser/repository-to-mirror.git",
                @"clone --mirror -- https://github.com/exampleuser/repository-to-mirror.git"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_posix, commands, 50, false));
        }

        [Fact]
        public void MsDosStyleCommand1()
        {
            var commands = new[]
            {
                @"add *.c",
                @"clone /reference:my2.6 git://git.kernel.org/pub/scm/.../linux-2.7 my2.7",
                @"clone git@github.com:whatever",
                @"clone git@github.com:whatever folder-name",
                @"clone /mirror https://github.com/exampleuser/repository-to-mirror.git"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msDos, commands, 50, false));
        }

        [Fact]
        public void MsStdStyleCommand1()
        {
            var commands = new[]
            {
                @"add *.c",
                @"clone -reference my2.6 git://git.kernel.org/pub/scm/.../linux-2.7 my2.7",
                @"clone git@github.com:whatever",
                @"clone git@github.com:whatever folder-name",
                @"clone -mirror https://github.com/exampleuser/repository-to-mirror.git",
                @"clone git@github.com:whatever -c A -c B -c C",
                @"clone -mirror:True https://github.com/exampleuser/repository-to-mirror.git"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50, false));
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedMember.Local

    }
}