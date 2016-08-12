squirrel_exe =  Dir.glob("packages/squirrel.windows.*/tools/Squirrel.exe")[0]

system(squirrel_exe + " " + ARGV.join(" "))

