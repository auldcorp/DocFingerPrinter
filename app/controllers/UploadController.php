<?php
namespace Napkins;

require_once(__DIR__ . '/class_template.php');

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;

Class UploadController {
	public function uploadAction(Request $request, Application $app) {
		return Response('YOLOOOOOOOOOO');
	}
	public function uploadView(Request $request, Application $app)
	private function upload(Application $app) {

	}
}
?>
