<?php
use Silex\WebTestCase;

require __DIR__ . '/../../../vendor/autoload.php';

class TestController extends WebTestCase{

	public function createApplication(){
		
		$app_env = 'test';

		return require __DIR__ .'/../../../index.php';
	}

	public function setUp(){
		parent::setUp();
	}

	public function testStatusCode(){
		$client = $this->createClient();
		$crawler = $client->request('GET', '/ajksgdyfjasdfuyash');

		$this->assertEquals(404, $client->getResponse()->getStatusCode(), 'Unwanted page');

		$crawler = $client->request('GET', '/');

		//if(!$client->getResponse()->isSuccessful()) var_dump($crawler->text());
		$this->assertEquals(200,$client->getResponse()->getStatusCode(), 'Welcome page down');
	}

}
