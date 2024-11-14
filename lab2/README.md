# File transfer via TCP with data transfer rate calculation

# Description
A program that implements a protocol for transferring any file from one computer to another on a local network. Implemented server and client. 
- Server receive files. 
- Client send files.
- The server displays the data transfer rate every 3 seconds.
- The server write files in directory uploads which will be located with executable file.

# Get started

- For start clone repository
``` bash
git clone https://github.com/MotorinIlya/networks.git
```
- if you want switch on server:
``` bash
./lab2 server {port}
```
where 
- {port} is the port on which the server will listen to clients



- if you want switch on client
``` bash
./lab2 client {path with filename} {ip server} {port server}
```
where 
- {path with filename} is absolute path to file.
- {ip server} is local ip server
- {port server} is port which server listen clients