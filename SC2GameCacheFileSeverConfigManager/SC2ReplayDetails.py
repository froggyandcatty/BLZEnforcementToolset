#! python2
from distutils.sysconfig import get_python_lib
import sys
import os
import s2protocol
if __name__ == '__main__':
	path = sys.argv[1]	
	os.system("py -2 " + get_python_lib() + "\s2protocol\s2_cli.py \"" + path + "\" --details")
	print status, output