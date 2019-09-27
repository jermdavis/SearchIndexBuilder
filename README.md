# SearchIndexBuilder
A tool for rebuilding search indexes from outside the Sitecore web app - for very long-running builds...

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

If you've dealt with older Sitecore projects that use large search indexes, then you've almost certainly hit the
issue of "My search index rebuild takes so long, that the IIS process recycles before it finishes"...

This tool tries to help with that by running the indexing operation from outside the ASP.Net website process.

Build the console application here, and then make use of the options it provides...

## Deploying the endpoint

`SearchIndexBuilder.App.exe Deploy -d <your website folder>`

This will deploy the endpoint `.aspx` file that allows the index builder tool to ineract with your Sitecore index.

## Setting up some config

`SearchIndexBuilder.App.exe Setup -u <url of the endpoint> -d <database> [-q <query for items>] [-c <config file name>]`


## Running an index build

`SearchIndexBuilder.App.exe Setup [-c <config file>][-o <output Every X items>] [-r <retries in case of error>]`