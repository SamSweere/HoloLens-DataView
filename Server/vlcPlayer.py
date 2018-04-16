import vlc
import time
import os

#repeat the player
instance = vlc.Instance('--input-repeat=-1')
i = 0

#set the music path
path = 'Music/'

#create a playlist
playlist = []
player = instance.media_player_new()

#load the playlist
def loadPlaylist():
    folder = os.listdir(path)
    for song in folder:
         playlist.append( path + str(song))

#setup the next song
def setupSong():
    global i
    if (i < len(playlist)) and (i>=0):
        song = playlist[i]
        media=instance.media_new(song)
        media.get_mrl()
        player.set_media(media)
    else:
        i = 0
        setupSong()

#play a song
def play():
    player.play()
    playing = set([1,2,3,4])
    time.sleep(1)
    duration = player.get_length()/1000
    mm, ss = divmod(duration,60)

#stop a song
def stop():
    player.stop()

#go to the next song
def next():
    global i
    i = i+1
    setupSong()
    play()

#go to the previous song
def previous():
    global i
    i-=1
    setupSong()
    play()

#pause a song
def pause():
    player.pause()

#volume up
def volUp():
    v = player.audio_get_volume()
    player.audio_set_volume(v+10)

#volume down
def volDown():
    v = player.audio_get_volume()
    player.audio_set_volume(v-10)

def main():
    loadPlaylist()
    setupSong()
