import subprocess
import sys
import setup_util
import os

def start_debug():
	class Args():
		database_host = "localhost"
		name = "websharper-sqlprovider-warp-pg-win"
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
    setup_util.replace_text("websharper-sqlprovider/Src/Db.fs", "Host=.*", "Host=" + args.database_host + ";")
    subprocess.check_call("powershell -Command .\\setup_win.ps1 start " + webhost, cwd="websharper-sqlprovider", stderr=errfile, stdout=logfile)
    return 0
  except subprocess.CalledProcessError:
    return 1

def stop(logfile, errfile):
  if os.name != 'nt':
    return 0
  
  subprocess.check_call("powershell -Command .\\setup_win.ps1 stop " + webhost, cwd="websharper-sqlprovider", stderr=errfile, stdout=logfile)
  return 0