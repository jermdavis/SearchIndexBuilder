<pre>
  _____                     _     _____           _
 / ____|                   | |   |_   _|         | |
| (___   ___  __ _ _ __ ___| |__   | |  _ __   __| | _____  _
 \___ \ / _ \/ _` | '__/ __| '_ \  | | | '_ \ / _` |/ _ \ \/ /
 ____) |  __/ (_| | | | (__| | | |_| |_| | | | (_| |  __/>  <
|_____/ \___|\__,_|_|  \___|_| |_|_____|_| |_|\__,_|\___/_/\_\
                                                       Builder
</pre>
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

If you've dealt with older Sitecore projects that use large search indexes, then you've almost certainly hit the
issue of "My search index rebuild takes so long, that the IIS process recycles before it finishes"...

This tool tries to help with that by managing the indexing operation from outside the ASP.Net website process. If
something causes the web app to recycle, this tool will detect the error and back off before retrying and continuing
the process. You can also stop the process and restart it later if necessary.

It will also try to manage errors raised by the Sitecore indexing process - but this behaviour is somewhat limited
by the data returned from an indexing job by Sitecore. As far as I can tell, most internal failures return a message
that still looks like success - even if, say, a computed field threw an exception. So you will need to check
your crawler log to investigate whether any errors which were unreported by Sitecore occurred.

This hasn't been exhaustively tested, as it was something I hacked together to help with a work problem. But it's been
tried against both Solr and Lucene indexes, with Sitecore v7.1, v7.2 & v9.0 - but in theory it should work with V7.0 and up.

Grab a [release](/jermdavis/SearchIndexBuilder/releases) and then make use of the options it provides...

## Step 1: Deploying the endpoint

The first step in running the tool is to deploy the special endpoint it uses into your sitecore application. The tool can
do this with the `Deploy` verb:

`SearchIndexBuilder.App.exe Deploy -w <your website folder> [-o] [-t <token>]` 

The parameters are:

* `-w` / `-website` (Required, string) : The Sitecore website's web root folder. This is where the endpoint file will be deployed to.
  Remember to put quotes around this string if it includes spaces. e.g. `-w "c:\inetpub\wwroot\mysite\Website"`
* `-o` / `-overwrite` (Optional) : By default the tool will not overwrite an existing endpoint file if one is found. If you do want
  to overwrite the existing file, add this parameter.
* `-t` / `-token` (Optional, string) : To do anything, requests to the endpoint file must include a security token. By default a
  random Guid will be used. But if you want to specify your own, then pass it with this parameter. Remember to use quotes if it includes
  spaces. The value used will be echoed to the screen, as you will need it in the next step. e.g. `-t MySuperSecretToken94!`

You must complete this step before proceeding.

## Step 2: Setting up some config

To run an indexing job, the tool relies on a JSON file which specifies job configuration and settings. You can write this file manually if
you want to, but the tool will generate it for you using the `Setup` verb.

`SearchIndexBuilder.App.exe Setup -u <url of the endpoint> -d <database> -t <token> [-q <query for items>] [-c <config file name>] [-o]`

The parameters are:

* `-u` / `-url` (Required, string) : The base url for the website you want the tool to talk to. It should be the web path that matches the
  website folder you specified in the deploy step above. You do not need to specify the name of the endpoint file. That will be added by
  tool. e.g. `-u https://mysite.domain.com/`
* `-d` / `-database` (Required, string) : The Sitecore database name that you want to extract item data from. When the tool generates the
  list of items to process it will use this for the query. e.g `-d web`
* `-t` / `-token` (Required, string) : You must supply the same security token that you set up above when you deployed the endpoint. If you
  you supplied your own you can just type it here. If you let the tool generate one, you will need to copy it from the output that the
  previous step generated. e.g. `-t MySuperSecretToken94!`
* `-q` / `-query` (Optional, string) : By default, the tool will reindex all the items in the database you specify. If you omit this parameter the code
  will go directly to the underlying Items database table for maximum performance. If you specify a query then this will be run against the Sitecore
  database object for the database name you provided. That means that a query will have more of a performance hit on the target server. Take care when
  using this option, as a query like `\\*` will potentially process many thousands of items. e.g. `-q "/sitecore/Content/*//[@@templatename='Homepage']"`
* `-c` / `-config` (Optional, string) : By default the tool will write the results of this operation to a file called `config.json` in the current folder.
  If you want to write to a different name or location, specify it with this parameter. e.g. `-c mySite.json`
* `-o` / `-overwrite` (Optional) : By default the tool will not overwrite an existing config file if one is found. If you do want
  to overwrite the existing file, add this parameter.

The config file will include all the Sitecore indexes defined on your site by default. If you only want to build certain indexes, use a text editor to
remove the unwanted ones from the JSON data. Just remember not to break the format of the file.

## Step 3: Running an index build

To start the process of re-indexing, you use the `Index` verb. This will take a config file created by the previous step, and process each of the content
items it specifies. Using the endpoint you've deployed, the tool will ask Sitecore to reindex each of the items, using each of the indexes
you have specified. 

`SearchIndexBuilder.App.exe Index [-c <config file>][-o <output Every X items>] [-r <retries in case of error>] [-p <ms to pause for>]`

The parameters are:

* `-c` / `-config` (Optional, string) : The tool will try to load configuration from a file named `config.json` in the current directoy by default. If you want to use
  a different config file, specifiy it with this parameter. e.g. `-c ..\testing\mySite.json`
* `-o` / `-outputEvery` (Optional, integer) : The code tries to estimate the time remaining for the rebuild operation by using a rolling average over the last 50 items that
  have been processed. This flag specifies how often the estimates should be displayed on the screen. It defaults to once every 10 items processed. e.g. `-o 35`
* `-r` / `-retries` (Optional, integer) : If an indexing call to the endpoint returns an error (either because the endpoint could not be accessed, or because Sitecore returned
  an error) then the operation will be retried this number of times before the tool decides the error is permenant and moves on to the next item. The tool will back off an
  increasing amount after each error. The default value is five retries. e.g. `-r 10`
* `-p` / `-pause` (Optional, integer) : If you want to lower the impact of the indexing process on your target server then you can use this
  parameter to add a pause between each item indexing request. The value is in milliseconds. e.g. `-p 250`
* `-t` / `-timeout` (Optional, integer) : The default timeout for HTTP operations with Sitecore is 60 seconds. You can specify a longer timeout (in seconds) using this flag.

You can stop the tool safely with `Ctrl-C`. It will finish its current operation, and then end. The current state (specifically what items are left to process, and what errors
have been recorded - both transient and permenant) will be written to disk in the config file. The previous state of the config fill will be preserved in a backup file named with
the format `backup-<date>-<time>-<config>.json` so that you can revert to this previous state if necessary.

The updated config is also saved to disk when the tool finishes normally - giving a record of items which caused problems.

## Step 4: Retrying errored items

If you have a config file with errors recorded in it, and you want to re-process those items, you can use the `retry` verb to generate a new config file
from the processed one. It will clear the processed items, elapsed time and attempts count data, and add any errors into the items list. You can then re-run
the `index` verb.

`SearchIndexBuilder.App.exe Deploy [-s <source config file>] [-t <target config file>] [-o]` 

The parameters are:

* `-s` / `-source` (Optional, string) : The config file you've already run, that you want to retry the errors from. Defaults to `config.json`. e.g. `-s currentSite.json`
* `-t` / `-target` (Optional, string) : The new config file you want to save the results to. Defaults to `retry-config.json`. e.g. `-t "retry currentSite.json"`
* `-o` / `-overwrite` (Optional) : If the target file exists, it can be overwritten if this flag is provided.

## Step 5: Removing the endpoint

Once you're finished, you should remove the endpoint file from the target website. You can do that by just deleting the file, but the tool
can do this for you with the `Remove` verb.

`SearchIndexBuilder.App.exe Remove -w <your website folder>`

The parameters are:

* `-w` / `-website` (Required, string) : The Sitecore website's web root folder. This is where the endpoint file will be removed from.
  Remember to put quotes around this string if it includes spaces. e.g. `-w "c:\inetpub\wwroot\mysite\Website"`

## Other parameters

The system also supports some global parameters, which will affect all of the verbs:

* `-a` / `-attach` (Optional) : Causes the processing to pause between parsing the command line options and starting any processing.
  This allows you to attach a debugger if you need to.
* `-f` / `-fake` (Optional) : Makes the code use a "fake" object as the endpoint proxy class - allowing it to process some data without a
  connection to Sitecore being possible.