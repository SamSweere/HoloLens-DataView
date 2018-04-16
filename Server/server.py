# Student: Sam Sweere s4403142, Julian Besems s4783751

#dependency: https://github.com/csparpa/pyowm
import pyowm

import datetime
import time
import thread
#import vlcPlayer as vlc
from socket import *

#setup openweather key:
owm = pyowm.OWM('2be2a5b389c1277b097bce6a3e7aff80')

# Search for current weather in Nijmegen
observation = owm.weather_at_coords(51.825372, 5.868212)
# Make the weather object
w = observation.get_weather()


def getWeatherInfo():
	temp = (str(w.get_temperature('celsius')).split("'temp': ")[1]).split(",")[0]
	humid = str(w.get_humidity())
	status = str(w.get_status())

	message = "Status:\t\t\t\t" + status + "\n" + "Temperature:\t" + temp + "C" + "\n" + "Humidity:\t\t\t" + humid + "%" + "\n"
	return message

def makeSendPackage():
	#Make the string to send
	toSend = ""
	#send the current time in the hour, minutes and seconds format
	toSend += "time#" + datetime.datetime.now().strftime('%H:%M:%S')
	#add a ';' to split on at the client
	toSend += ";"
	#send the counter
	toSend += "weather#" + getWeatherInfo()
	#add the last newline
	toSend += "\n"
	#send the current time in the hour, minutes and seconds format
	return str.encode(toSend)

def sendInfo(connectionSocket):
	try :
		while(True):
			#make the information to send and send it
			connectionSocket.send(makeSendPackage())
			#wait one seccond to send the next time
			time.sleep(0.5)
	except error:
		pass
	return

def music(indata):
        print(indata+" from music")
        if indata == "play":
		print("starting " + indata)	
 #               vlc.play()
	elif indata == "stop":
#		print("starting play")
		print("starting " + indata)
	elif indata == "pause":
#		vlc.pause()
		print("starting " + indata)
	elif indata == "next":
#		vlc.next()
		print("starting " + indata)
	elif indata == "previous":
#		vlc.previous()
		print("starting " + indata)
	elif indata == "volUp":
#		vlc.volUp()
		print("starting " + indata)
	elif indata == "volDown":
#		vlc.volDown()
		print("starting " + indata)


def interpret(inp):
	try:
		val = inp.split('#')
		val[-1] = val[-1][:-1]
		print(type(val[1]))
		# print(begin)
		# print(end)
		print(val)
		if (val[0] == "music"):
			print(val)        	
			print(val[1] + " from if")
			music(val[1])
		else :
			print(inp)
	except ValueError:
		print(inp)

def recieveInfo(connectionSocket):
	try:
		while True:
			sentence = connectionSocket.recv(1024)
			# while the other side hasn't closed the socket
			inputSentence = None
			while sentence:
				#try to decocde
				try:
					inputSentence = str(sentence.decode("utf-8"))#.encode("latin_1")
					if(inputSentence.endswith("\n")):
						break
				except UnicodeDecodeError:
					#if decoding fails break
					break
			if(inputSentence != None):
				interpret(inputSentence[:-1])
	except error:
		pass
	return

def serverStart():
	serverPort = 20000
	# build TCP socket
	serverSocket = socket(AF_INET, SOCK_STREAM)
	serverSocket.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)
	# bind it to port
	serverSocket.bind(('', serverPort))
	# listen (clients will start queuing up)
	serverSocket.listen(1)

	print('The server is ready to send')
	while 1:

		# the is waiting for clients to connect/ processes them one at a time
		connectionSocket, addr = serverSocket.accept()
		print('Processing client ', addr)
		#start a new thread to send the data
		thread.start_new_thread (sendInfo, (connectionSocket, ) )
		#start a new thread to recieve the data
		thread.start_new_thread (recieveInfo, (connectionSocket, ) )
	print('Client closed ', addr)

	# close socket
	connectionSocket.close()
	return

#vlc.main()
serverStart()
