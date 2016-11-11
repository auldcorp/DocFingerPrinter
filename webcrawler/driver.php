<?php
date_default_timezone_set('America/New_York');
require_once __DIR__.'/vendor/autoload.php';
//$loader = require 'vendor/autoload.php';
//$loader->add('Acme\\Test\\', __DIR__);
//$loader->add('AppName', __DIR__.'/../src/');

include("libs/PHPCrawler.class.php");
//include("vendor/jenssegers/");
// It may take a whils to crawl a site ...
set_time_limit(10000);

// Inculde the phpcrawl-mainclass
use \Jenssegers\ImageHash\ImageHash;


// Extend the class and override the handleDocumentInfo()-method
class MyCrawler extends PHPCrawler
{
    public $tempdir = "NULL";
    public $db = NULL;

    function handleDocumentInfo($DocInfo)
    {
        $pdoDebug = true;
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

        // If file is an image
        if(strcmp(substr($DocInfo->content_type, 0, 6), "image/") == 0) {

            // Generate filename and save file
            $tempfile = $this->tempdir.rand(1000, 9999);
            copy($DocInfo->url, $tempfile);

            //Get Hash
            $hasher = new ImageHash(NULL, ImageHash::DECIMAL);
            $temp_hash = $hasher->hash($tempfile);
            echo $temp_hash.$lb;

            //Search for all similar hashes
            $sql = "SELECT images.email, users.name, images.date, images.orig_name, images.hash FROM images INNER JOIN users ON (images.email = users.email) WHERE BIT_COUNT(hash ^  :temp_hash) < 2";
            $stmt = $this->db->prepare($sql);
            $stmt->bindValue('temp_hash', $temp_hash);

            try {
                $stmt->execute();
            } catch (PDOException $exception) {
                // unlike mysql/mysqli, pdo throws an exception when it is unable to connect
                echo 'There was an error connecting to the database!\n';
                if ($pdoDebug) {
                    // pdo's exception provides more information than just a message
                    // including getFile() and getLine()
                    echo $exception->getMessage();
                }
            }

            $db_hashes = $stmt->fetchAll();

            // send notification for each hash
            $first = TRUE;
            foreach($db_hashes as $row) {
                //store in database
                if ($first)
                {
                    $sql = "INSERT INTO napkins.found (address_hash, hash, address, date) VALUES (:address_hash, :temp_hash , :address, :date)";
                    $stmt = $this->db->prepare($sql);
                    $stmt->bindValue('address_hash', hash('md5', $DocInfo->url));
                    $stmt->bindValue('temp_hash', $temp_hash);
                    $stmt->bindValue('address', $DocInfo->url);
                    $stmt->bindValue('date', date('Y/m/d'));

                    try {
                        $stmt->execute();
                    } catch (PDOException $exception) {
                        echo 'There was an error connecting to the database!\n';
                        if ($pdoDebug) {
                            echo $exception->getMessage();
                        }
                    }
                }

                $sql = "SELECT hash FROM napkins.found WHERE address_hash like :address_hash";
                try {
                    $stmt->execute();
                } catch (PDOException $exception) {
                    echo 'There was an error connecting to the database!\n';
                    if ($pdoDebug) {
                        echo $exception->getMessage();
                    }
                }
                $db_hashes = $stmt->fetchAll();

                // Calculate Grade
                $grades = array('A', 'B', 'C', 'D');
                echo "\r\n".$row['hash']." ".$db_hashes[0]['hash']."\r\n";
                $distance = $hasher->distance($row['hash'], $db_hashes[0]['hash']);
                echo "\r\n".$distance."\r\n";
                echo "\r\n".gmp_hamdist($row['hash'], $db_hashes[0]['hash'])."\r\n";

                // notify owners
                $to      = $row['email'];
                $subject = 'Potential Image Match';
                $headers = 'From: Auld Corp <do-not-reply@auldcorporation.com>' . "\r\n" .
                    'X-Mailer: PHP/' . phpversion();
                $message = "Hello ".$row['name'].","."\r\n".
                    "\r\n".
                    "We have found a potential image match for your image \"".$row['orig_name']."\" at:\r\n".
                    "\r\n".
                    $DocInfo->url."\r\n".
                    "\r\n".
                    "With a match grade of ".$grades[$distance]"\r\n".
                    "\r\n".
                    "Best regards,"."\r\n".
                    "The Auld Corporation"."\r\n";

                echo $message;




                //mail($to, $subject, $message, $headers);

                $first = FALSE;
            }


            // Delete File
            unlink($tempfile);
        }

        echo $lb;

        flush();
    }
}

// Setup database connection information

// Now, create a instance of your class, define the behaviour
// of the crawler (see class-reference for more options and details)
// and start the crawling-process.

function spawn_crawler($url, $db)
{
    $crawler = new MyCrawler();
    $crawler->db = $db;
    $crawler->tempdir = "/tmp/napkin_collector/".getmypid().".d/";
    if (!mkdir($crawler->tempdir, 0777, true))
        echo "Error Creating tempdir: ".$crawler->tempdir."\n";

    // URL to crawl
    $crawler->setURL($url);

    // Receive content of files with content-type "text/html" so we can find links
    $crawler->addContentTypeReceiveRule("#text/html#");

    // Recieve images so we can try to fingerprint them
    $crawler->addContentTypeReceiveRule("#image/[a-zA-Z\-]{2,10}#");
    // Store and send cookie-data like a browser does
    $crawler->enableCookieHandling(true);
    $crawler->setUrlCacheType(PHPCrawlerUrlCacheTypes::URLCACHE_SQLITE);
    $crawler->setFollowMode(0);
    // Set the traffic-limit to 1 MB (in bytes,
    // for testing we dont want to "suck" the whole site)
    $crawler->setTrafficLimit(1000 * 1024);

    // Go crawler Go!!
    //$crawler->goMultiProcessed(5);
    $crawler->go();

    if (!rmdir($crawler->tempdir))
        echo "Error Deleting tempdir: ".$crawler->tempdir.$lb;

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

    return $report->links_followed;
}



// Get URLs from config
$sql = "SELECT config_value FROM napkins.crawler WHERE config_key LIKE 'url'";
$db = new PDO('mysql:host=localhost;dbname=napkins;charset=utf8mb4', 'napkin_collector', '1DCvnvnUoRbFSQKu7zBXwxG');
$stmt = $db->prepare($sql);

try {
    $stmt->execute();
} catch (PDOException $exception) {
    // unlike mysql/mysqli, pdo throws an exception when it is unable to connect
    echo 'There was an error connecting to the database!\n';
    if ($pdoDebug) {
        // pdo's exception provides more information than just a message
        // including getFile() and getLine()
        echo $exception->getMessage();
    }
}
$urls = $stmt->fetchAll();

foreach($urls as $url) {
    spawn_crawler($url['config_value'], $db);
}

?>
