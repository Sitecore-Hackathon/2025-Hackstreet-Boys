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
3. Extract the Archive such that the extraction dialog looks like - 
![Extraction] (docs/images/1.png?raw=true "Extraction")
4. Navigate to the directory at C:\SitecoreHackathon25\ch-cli-win-x64.1.1.75 and open in Commandline as an Administrator.
3. Run the following command in CLI -- `ch-cli`

> Setup OAuth Client in Sitecore Content Hub

Please follow these instructions for setting up OAuth Client inside Content Hub https://doc.sitecore.com/ch/en/users/content-hub/create-an-oauth-client.html



## Installation instructions

1. Open the following directory C:\SitecoreHackathon25\ch-cli-win-x64.1.1.751 in Windows Explorer.
2. Copy the Folder "Sitecore.CH.Cli.Plugin.HackStreet" from the current repo at the location /src/cliplugin to the folder "C:\SitecoreHackathon25\ch-cli-win-x64.1.1.751\plugins"
3. The Directory Structure of the plugins should look like this after installation 
![After Installation](docs/images/after-installation.png?raw=true "After Installation")

### Configuration
1. Open Commandline with the following location as an Administrator: C:\SitecoreHackathon25\ch-cli-win-x64.1.1.751. 
2. Login to a Content Hub instance using the following CLI Command - 
ch-cli endpoint add --name [CONTENTHUB-INSTANCE-NAME] --url [CONTENTHUB-INSTANCE-URL] --client-id [CLIENT-ID] --client-secret [CLIENT-SECRET] --redirect-uri [REDIRECT-URL]
    - CONTENTHUB-INSTANCE-NAME - Name of your Content Hub Instance - like - ContentHub-Sandbox
    - CONTENTHUB-INSTANCE-URL - The URL of your Content hub Instance - like - https://ph-sandbox.sitecoresandbox.cloud/
    - CLIENT-ID - The OAuth Client ID generated - Like - CLIClient 
    - CLIENT-SECRET - The OAuth Client Secret - like - X62XXXXX-XXXX-4738-bXXX-1XXX20aXXXXX
    - REDIRECT-URL - The URL of redirection in http - like - http://localhost:9500/
3. We have obfuscated text for security reasons, although, this is how our command looks like in the Commandline - 
![CLI Login Command](docs/images/CLI-Login-Command.png?raw=true "CLI Login Command")
4. Press Enter - and it will open up a login screen for access in your browser 
![Browser Login](docs/images/browser-login.png?raw=true "Browser Login") 
5. Provide the credentials and say grant access 
![Authorize](docs/images/authorize.png?raw=true "Authorize")
6. It would say it is successful now. 

![Browser Login](docs/images/success.png?raw=true "Browser Login")  
7. 

## Usage instructions

###Export Command Usage Instructions
1. Create a Directory to export an Excel file with details of the Assets. Lets create one called "assetstoexport" in C Drive.
![Export Directory](docs/images/assets-to-export.png?raw=true "Export Directory")  
2. After the above Login is successful in the configurations, fire the following command in the command like
`ch-cli hackstreet exportasset --query AssetMediaToAsset=M.AssetMedia.jpg --location C:\assetstoexport\Asset.xlsx --fields "Title|Description|AssetTypeToAsset|AssetMediaToAsset ` It looks like 
![Command in CommandLine](docs/images/Command-in-CommandLine.png?raw=true "Command in CommandLine")
3. Press Enter and it would execute the command
![Execution One](docs/images/Execution-1.png?raw=true "Execution One")

![Execution Two](docs/images/Execution-2.png?raw=true "Execution Two")

![Execution Three](docs/images/Execution-3.png?raw=true "Execution Three")
4. Notice above, that the messages which are Info, are in blue, Success steps are in Green and the ones which are failed because of required fields not in Content Hub are in RED. Also, the items selected via the query are 26 while the items which are exported are 23. 
5. This Excel file can now be Imported in a different Content Hub instance after logging into the other instance using the Content Hub content CLI Command `ch-cli content import -s C:\assetstoexport\Asset.xlsx`

###Create Email Template Command Usage Instructions

## Comments

- Reference Links:
1. Creating a Custom Plugin - https://doc.sitecore.com/ch/en/developers/cloud-dev/create-a-plugin-to-register-a-command.html
2. Email Template Creation - https://doc.sitecore.com/ch/en/developers/cloud-dev/notifications-client.html#create-an-email-template

If you'd like to make additional comments that is important for your module entry.