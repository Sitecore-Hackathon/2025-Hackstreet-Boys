![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")
# Sitecore Hackathon 2025
  
## Team name
⟹ HackStreet Boys

## Category
⟹ Free for all - Create your own idea!


## Description

⟹ Our Idea: Extending Sitecore Content Hub CLI with Plugins 

- Module Purpose:
	Export Assets from Content Hub using Content Hub CLI
	Create Email Template in Content Hub using Content Hub CLI

- What Problem does it solve?
At present, we have found two opportunities with Sitecore Content Hub.
1. If you want to move Assets from One instance of Content Hub to another, there is no straight forward way available. Either you do this manually, or PowerShell using Rest API, or any kind of Middleware. But, the thing is, you need to write a piece of code and do the neccessary transformation to generate the public link for the image that can be used while importing asset to another tenant. 
2. If you want to create an Email Template in Sitecore Content Hub, again, there is no Create button to create it directly. You need to write a console application to create an Email template, while later you can change the Subject and Body, although Name, Label and Variables remain the same.

- How does this module solve it?
We have created a Custom Plugin, and developed Two Commands as a part of the Sitecore Hackathon 2025, to take care of both the above - Exporting Assets from Content Hub and Creating an Email Template in Content Hub - using Sitecore Content Hub CLI.

## Video link

⟹ [Replace this Video link](#video-link)

## Pre-requisites and Dependencies

> Installation of Sitecore Content Hub CLI is required - https://doc.sitecore.com/ch/en/developers/cloud-dev/install-the-content-hub-cli.html 

For simplicity, we have provided the Steps here:
1. Download the latest version of Content Hub CLI from the GitHub - https://github.com/Sitecore/content-hub-cli/releases/tag/1.1.75 For Windows, users Download the Zip directly using the link: https://github.com/Sitecore/content-hub-cli/releases/download/1.1.75/ch-cli-win-x64.1.1.75.zip
2. Create a Directory C:\SitecoreHackathon25
3. Extract the Archive such that the extraction dialog looks like - docs/images/1.png?raw=true
4. Navigate to the directory at C:\SitecoreHackathon25\ch-cli-win-x64.1.1.75 and open in Commandline as an Administrator.
3. Run the following command in CLI -- `ch-cli`

> Setup OAuth Client in Sitecore Content Hub

Please follow these instructions for setting up OAuth Client inside Content Hub https://doc.sitecore.com/ch/en/users/content-hub/create-an-oauth-client.html



## Installation instructions

1. Open the following directory C:\SitecoreHackathon25\ch-cli-win-x64.1.1.751 in Windows Explorer.
2. Copy the Folder "Sitecore.CH.Cli.Plugin.HackStreet" from the current repo at the location /src/cliplugin to the folder "C:\SitecoreHackathon25\ch-cli-win-x64.1.1.751\plugins"
3. The Directory Structure of the plugins should look like this after installation (docs/images/after-installation.png?raw=true "After Installation")

### Configuration
1. Open Commandline with the following location as an Administrator: C:\SitecoreHackathon25\ch-cli-win-x64.1.1.751. 
2. Login to a Content Hub instance using the following CLI Command - 
ch-cli endpoint add --name [CONTENTHUB-INSTANCE-NAME] --url [CONTENTHUB-INSTANCE-URL] --client-id [CLIENT-ID] --client-secret [CLIENT-SECRET] --redirect-uri [REDIRECT-URL] 
3. We have obfuscated text for security reasons, although, this is how our command looks like in the Commandline - (docs/images/CLI-Login-Command.png?raw=true "Hackathon Logo")
4. Press Enter - and it will open up a login screen for access in your browser (docs/images/browser-login.png?raw=true "Browser Login") 
5. Provide the credentials and say grant access (docs/images/authorize.png?raw=true "Authorize")
6. It would say it is successful now. (docs/images/success.png?raw=true "Browser Login")  

## Comments

- Reference Links:
1. Creating a Custom Plugin Export 
2. Email Template Creation - https://doc.sitecore.com/ch/en/developers/cloud-dev/notifications-client.html#create-an-email-template

If you'd like to make additional comments that is important for your module entry.