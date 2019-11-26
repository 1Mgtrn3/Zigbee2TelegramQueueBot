# Zigbee2TelegramQueueBot

Zigbee2TelegramQueueBot is a Telegram bot that keeps track of a Zigbee door sensor using MQTT. It can also create a queue for people willing to get into the room, controlled by the door sensor.

## Notable features:
- The bot is able to **use a webhook without an SSL certificate**. Though there is little requirement listed **[below](#setting-up-a-webhook-without-an-ssl-certificate)**.
- The bot supports localization using .resx files
- There are 2 modes for the menu: text commands and reply_markup buttons.
- The bot can be configured to use both HTTP and SOCKS5 proxies. More on that [here](#setting-up-a-proxy).
- The bot can either provide notifications for the sensor status change (**simple mode**) or create a queue for the people willing to get into the room (**regular mode**).

------------
## Bot configuration
Configuring the bot using Appsettings.json is kind of complicated currently. Please keep in mind that the "production-tested" setup **roughly** looks like this (the **ignored** parameters are **not shown**, but you **still need to keep them** in appsettings.json):

```json
"BotConfiguration": {
    "Language": "Russian",
    "SimpleMode": true,
    "ProxyMode": "HTTP",
    "ExternalURL": "",
    "TimeLimitMinutes": "20",
    "HTTPProxy": {
      "HTTPHost": "host",
      "HTTPPort": "80",
      "HTTPUser": "",
      "HTTPPassword": ""
    },
    "NgrokPath": "/NgrokDir/ngrok",
    "NgrokArguments": "http 8443",
    "AttempstLimit": "5"
  },
  "LockTrackerConfiguration": {
    "IsLockTrackerInstalled": true,
    "TcpServerIp": "172.30.3.103",
    "TcpServerPort": "1883",
    "ClientId": "TelegramBot",
    "MqttTopic": "zigbee2mqtt/123456789",
    "TestMode": false
  }
```
### Configuration prioritization, in short
First I want to discribe a heirarachy among the parameters of appsettings.json.
The more detailed setup instructions are **[here](#setup-instructions)** (in the bottom or this Readme)
Here is a series of tables which describe the hierarchy of the settings in the config:
#### Functionality-wise

| |"SimpleMode":true|"SimpleMode":false|
| ------ | ------ | ------ |
| "MenuMode" | Ignored|**Used**|

#### Webhook-wise
| |"ExternalURL":"https://xyz.io"|"ExternalURL":""|
| ------ | ------ | ------ |
| "NgrokPath" | Ignored|**Used**|
| "NgrokArguments" | Ignored|**Used**|

#### Proxy-wise
| |"ProxyMode":"NONE"|"ProxyMode":"HTTP"|"ProxyMode":"SOCKS5"|
| ------ | ------ | ------ | ------ |
| "SOCKS5Proxy" section | Ignored|Ignored|**Used**|
| "HTTPProxy" section | Ignored|**Used**|Ignored|
#### Door sensor-wise

| |"IsLockTrackerInstalled":"false"|"IsLockTrackerInstalled":"true"|
| ------ | ------ | ------ |
| "TestMode" | Ignored|**Used**|
| "TcpServerIp" | Ignored|**Used**|
| "TcpServerPort" | Ignored|**Used**|
| "ClientId" | Ignored|**Used**|
| "MqttTopic" | Ignored|**Used**|
#### Simulation or non-Zigbee sensor ("TestMode")
| |"TestMode":"true"|"TestMode":"false"|
| ------ | ------ | ------ |
| "TcpServerIp" | Ignored|**Used**|
| "TcpServerPort" | Ignored|**Used**|
| "ClientId" | Ignored|**Used**|
| "MqttTopic" | Ignored|**Used**|
------------
## Setup instructions
### Setting up a webhook without an SSL certificate
#### Purpose
First of all, this can be used for the tests and for some really small projects. But this is very useful in this particular case because you need to have the bot's server connected to the MQTT queue somehow, so you can host the whole thing locally and still have a webhook.

#### What makes it possible
This is achieved by using [Ngrok](https://ngrok.com/). Ngrok exposes local servers behind NATs and firewalls to the public internet over secure tunnels.
Zigbee2TelegramQueueBot runs ngrok via Process.Start and fetches a public URL from the [ngrok's tunnels page](http://127.0.0.1:4040/api/tunnels).
It is possible to use ngrok without an account for a limited period of time per session. However, you can create an account [here](https://dashboard.ngrok.com/signup) and get one public address with an unlimited amount of time per session. 

#### Setup
##### Ngrok account creation & authentication
In order to make this whole setup work, you need to download the ngrok archive from ngrok's dashboard, unzip it somewhere and authenticate using the key you got after ngrok account creation (just follow the steps described [here](https://dashboard.ngrok.com/auth)).
##### Editing the bot's config
Now open appsettings.json and change this setting according to the ngrok location:
```json
"NgrokPath": "/TelegramAPI/ngrok",
```
It is not recommended to change this setting:
```json
"NgrokArguments": "http 8443",
```
The reason for that is that you can't use any other port for that, because other ports supported by Telegram are behind the paywall in ngrok. In case you have a paid version of ngrok, you sure can change the port.

------------

### Setting up a proxy 
#### Proxy types
There are two types of proxies that can be used : 
- HTTP
- SOCKS5

#### Proxy type setup
You can set the proxy type by changing this line in appsettings.json.
```json
"ProxyMode": "NONE",
```

Possible values list:

| Value | Description |
| ------ | ------ |
| NONE | No proxy and proxy settings used |
| HTTP | Http proxy used. Configure it in the "HTTPProxy" section of appsettings.json |
| SOCKS5 | Socks5 proxy used. Configure it in the "SOCKS5Proxy" section of appsettings.json |

These values are determined by an enum called ProxyMode (ProxyMode.cs).

**Note**: user/password authentication for HTTP proxies is not implemented currently, however those parameters **SHOULD NOT** be excluded from the config.
Appsettings.json example:
```json
"ProxyMode": "HTTP",
"HTTPProxy": {
      "HTTPHost": "host",
      "HTTPPort": "80",
      "HTTPUser": "",
      "HTTPPassword": ""
    },
```

------------

### Setting up a Zigbee connection & queue 

The whole setup is inside this section of appsettings.json:
```json
"LockTrackerConfiguration": {
    "IsLockTrackerInstalled": true,
    "TcpServerIp": "172.30.3.103",
    "TcpServerPort": "1883",
    "ClientId": "TelegramBot",
    "MqttTopic": "zigbee2mqtt/123456789",
    "TestMode": true
  }
```

#### IsLockTrackerInstalled
This bot was created with 2 situations in mind:
1. Zigbee module is not used, and the whole thing is using reports from the users to track the room's status. This situation corresponds with 
```json 
 "IsLockTrackerInstalled" : false
 ```
2. Zigbee module is either installed or simulated. This situation corresponds with 
```json 
 "IsLockTrackerInstalled" : true
 ```
#### TcpServerIp
This is where you put the IP of your TCP Server (**not a Websocket one**). It is not recommended to type localhost or 127.0.0.1. Please use the IP for the subnet the server is in. 
#### TcpServerPort
This is where you put the Port of your TCP Server. It has to be int.
#### ClientId
A client ID. It is better to have one.
#### MqttTopic
A topic(queue) you have set your Zibgee up to.
#### TestMode
This is the most tricky part of the whole section.
This parameter is used to enable or disable a Zigbee simulation (alternatively, you can use it to connect any other door sensor using a separate service).
```json 
 "TestMode" : false
 ```
 This value makes the bot to use all the parameters mentioned above to connect to MQTT and to start processing messages from it.
 
 ```json 
 "TestMode" : true
 ```
 This value makes the bot to ignore all the MQTT parameters mentioned above (so "IsLockTrackerInstalled" would not be ignored) and to use the requests from  LockTrackerSimulationController as valid input. More on that in the next section of Readme.md.
 
 ------------
 
 ### Simulating the door sensor
 I have created this feature for the tests; however, it can be used to receive messages from the other type of sensor or from basically anywhere else.
 To enable the simlutaion you need to set this parameter in appsettings.json:
  ```json 
 "TestMode" : true
 ```
 After that you can start sending POST requests to /LockTrackerSimulation with a body looking like this:
  ```json 
{"contact":true, "linkquality": 200, "battery":100, voltage:3025}
 ```
 The model for that body is:
   ```c# 
public class LockTrackerMessage
    {
        public bool contact { get; set; }
        public int linkquality { get; set; }
        public int battery { get; set; }
        public int voltage { get; set; }
    }
 ```
* "contact":**true** - means the door is **closed**
* "contact":**false** - means the door is **open**
 

You can use this controller to pass the information from various sources (with the help of a custom web service). Just use the right body format.

------------

### Other BotConfiguration parameters
#### Language
Possible values list:

| Value | Description |
| ------ | ------ |
| Russian | Russian language for everything in the menus (logging is in English only) |
| English | English language for everything in the menus |

These values are determined by an enum called Language (Language.cs).
#### BotToken
This is where you put the bot token you got from [@BotFather](https://t.me/BotFather).
#### TimeLimitMinutes
This value is used to set the limit for the occupancy duration when a person tries to set a custom visit duration (this is only used in **Regular** mode, not a Simple one)
#### MenuMode
This parameter is not ignored only when the bot is not in the Simple mode:
  ```json 
"SimpleMode": false
 ```
Possible values list for this parameter:

| Value | Description |
| ------ | ------ |
| BUTTONS | Reply_markup buttons used to create a menu|
| TEXT | Text commands used to create a menu|

#### ExternalURL
This parameter is used to set up the webhook. This should look like 
* https://xyz.io
* https://xyz.io:443
* https://xyz.io:80
* https://xyz.io:88
* https://xyz.io:8443

In case this parameter is left empty, the bot tries to **use the automated ngrok** option described above.
