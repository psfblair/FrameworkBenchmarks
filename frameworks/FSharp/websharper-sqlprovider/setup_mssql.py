import subprocess
import sys
import setup_util
import os

def start_debug():
	class Args():
		database_host = ".\SQLEXPRESS"
		name = "websharper-sqlprovider-iis-mssql"
	os.chdir("..")
	start(Args(), sys.stdout, sys.stderr)
	
def start(args, logfile, errfile):
  if os.name != 'nt':
    return 1

  if 'iis' in args.name:
	webhost = 'iis'
  else:
    webhost = 'owin'

  try:
    subprocess.check_call("git checkout -f Src", cwd="websharper-sqlprovider", stderr=errfile, stdout=logfile)
    
    setup_util.replace_text("websharper-sqlprovider/Src/Db.fs", "Host=.*", "Server=" + args.database_host + ";")
    setup_util.replace_text("websharper-sqlprovider/Src/Db.fs", "Port=.*", "")
    setup_util.replace_text("websharper-sqlprovider/Src/Db.fs", "Username=.*", "User Id=benchmarkdbuser;")
    setup_util.replace_text("websharper-sqlprovider/Src/Db.fs", "Password=.*", "Password=B3nchmarkDBPass;")
    setup_util.replace_text("websharper-sqlprovider/Src/Db.fs", "Pooling=.*", "")
    setup_util.replace_text("websharper-sqlprovider/Src/Db.fs", "MaxPoolSize=.*", "max pool size=32767")
    setup_util.replace_text("websharper-sqlprovider/Src/Db.fs", "Common.DatabaseProviderTypes.POSTGRESQL", "Common.DatabaseProviderTypes.MSSQLSERVER")
    setup_util.replace_text("websharper-sqlprovider/Src/Db.fs", "ResolutionPath.*,", "CaseSensitivityChange=Common.CaseSensitivityChange.ORIGINAL,")
    setup_util.replace_text("websharper-sqlprovider/Src/World/Data.fs", "\[PUBLIC\].\[WORLD\]", "[dbo].[World]")
    setup_util.replace_text("websharper-sqlprovider/Src/World/Data.fs", "record.randomnumber", "record.randomNumber")
    setup_util.replace_text("websharper-sqlprovider/Src/World/WorldTests.fs", "\[PUBLIC\].\[WORLD\]", "[dbo].[World]")
    setup_util.replace_text("websharper-sqlprovider/Src/World/WorldTests.fs", "record.randomnumber", "record.randomNumber")
    setup_util.replace_text("websharper-sqlprovider/Src/Fortune/Data.fs", "\[PUBLIC\].\[FORTUNE\]", "[dbo].[Fortune]")

    subprocess.check_call("powershell -Command .\\setup_win.ps1 start " + webhost, cwd="websharper-sqlprovider", stderr=errfile, stdout=logfile)
    return 0
  except subprocess.CalledProcessError:
    return 1

def stop(logfile, errfile):
  if os.name != 'nt':
    return 0
  
  subprocess.check_call("powershell -Command .\\setup_win.ps1 stop " + webhost, cwd="websharper-sqlprovider", stderr=errfile, stdout=logfile)
  return 0