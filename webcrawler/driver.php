<?php

// It may take a whils to crawl a site ...
set_time_limit(10000);

// Inculde the phpcrawl-mainclass
include("libs/PHPCrawler.class.php");

// Extend the class and override the handleDocumentInfo()-method
class MyCrawler extends PHPCrawler
{
    public $tempdir = "NULL";

    function handleDocumentInfo($DocInfo)
    {
        // Just detect linebreak for output ("\n" in CLI-mode, otherwise "<br>").
        if (PHP_SAPI == "cli")
            $lb = "\n";
        else
            $lb = "<br />";

        // Print the URL and the HTTP-status-Code
        echo "Page requested: ".$DocInfo->url." (".$DocInfo->http_status_code.")".$lb;

        // Print the refering URL
        echo "Referer-page: ".$DocInfo->referer_url.$lb;

        // Print if the content of the document was be recieved or not
        if ($DocInfo->received == true)
            echo "Content received: ".$DocInfo->bytes_received." bytes".$lb;
        else
            echo "Content not received".$lb;


        // Now you should do something with the content of the actual
        // received page or file ($DocInfo->source), we skip it in this example

        // If file is an image
        if(strcmp(substr($DocInfo->content_type, 0, 6), "image/") == 0) {

            // Generate filename and save file
            $tempfile = $this->tempdir.rand(1000, 9999);
            copy($DocInfo->url, $tempfile);

            //compare hash

            //store in database

            //store site url

            //store abuse info for site

            //send notification

            // Delete File
            unlink($tempfile);
        }

        echo $lb;

        flush();
    }
}

// Now, create a instance of your class, define the behaviour
// of the crawler (see class-reference for more options and details)
// and start the crawling-process.

$crawler = new MyCrawler();
//$crawler->tempdir = "/home/kd8zev/napkin_collector/".getmypid()."";
$crawler->tempdir = "/tmp/napkin_collector/".getmypid().".d/";
if (!mkdir($crawler->tempdir, 0777, true))
    echo "Error Creating tempdir: ".$crawler->tempdir."\n";

// URL to crawl
$crawler->setURL("auldcorporation.com");

// Only receive content of files with content-type "text/html"
$crawler->addContentTypeReceiveRule("#text/html#");
// Recieve images so we can try to fingerprint them
$crawler->addContentTypeReceiveRule("#image/[a-zA-Z\-]{2,10}#");

// Ignore links to pictures, dont even request pictures
//$crawler->addURLFilterRule("#\.(jpg|jpeg|gif|png)$# i");

// Store and send cookie-data like a browser does
$crawler->enableCookieHandling(true);

// Set the traffic-limit to 1 MB (in bytes,
// for testing we dont want to "suck" the whole site)
$crawler->setTrafficLimit(1000 * 1024);

// Thats enough, now here we go
$crawler->go();

if (!rmdir($crawler->tempdir))
    echo "Error Deleting tempdir: ".$crawler->tempdir;

// At the end, after the process is finished, we print a short
// report (see method getProcessReport() for more information)
$report = $crawler->getProcessReport();

if (PHP_SAPI == "cli") $lb = "\n";
else $lb = "<br />";

echo "Summary:".$lb;
echo "Links followed: ".$report->links_followed.$lb;
echo "Documents received: ".$report->files_received.$lb;
echo "Bytes received: ".$report->bytes_received." bytes".$lb;
echo "Process runtime: ".$report->process_runtime." sec".$lb;
?>