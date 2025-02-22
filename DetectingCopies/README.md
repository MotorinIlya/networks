# Detecting copies of yourself on a local network

## Description

An application that discovers copies of itself on a local network using multicast UDP messaging. The application monitors the appearance and disappearance of other copies of itself on the local network and, when changes occur, displays a list of IP addresses of “live” copies.

- The multicast group address must be passed as a parameter to the application. 

- The application supports work in both IPv4 and IPv6 networks, selecting a protocol automatically depending on the transferred group address.

## Get started
- for start clone repository
``` bash
    git clone https://github.com/MotorinIlya/networks.git
```
- after start program enter the multicast group ip (for example 224.0.0.7 or FF02::1)
