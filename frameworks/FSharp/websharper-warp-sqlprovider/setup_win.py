import subprocess
import sys
import setup_util
import os

def start(args, logfile, errfile):
  if os.name != 'nt':
    return 1
  
  try:
  sed -i -e 's//Host='"${DBHOST};/" Src/Db.fs
    setup_util.replace_text("websharper-sqlprovider/Src/Db.fs", "Host=.*$", "Host=" + args.database_host)
    subprocess.check_call("powershell -Command .\\setup_win.ps1 start", cwd="websharper-sqlprovider", stderr=errfile, stdout=logfile)
    return 0
  except subprocess.CalledProcessError:
    return 1

def stop(logfile, errfile):
  if os.name != 'nt':
    return 0
  
  subprocess.check_call("powershell -Command .\\setup_win.ps1 stop", cwd="websharper-sqlprovider", stderr=errfile, stdout=logfile)
  return 0