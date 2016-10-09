<?php
namespace Napkins;

require_once(__DIR__ . '/class_template.php');

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;

Class UploadController {
	public function uploadAction(Request $request, Application $app) {
		$templating = new WebTemplate();
		$succeeded = [];
		$failed = [];
		foreach($request->files as $file) {
			if(preg_match("/^image\/[a-zA-Z|\-]{2,10}/",$file->getMimeType())) {
				$file->move("/srv/napkin/images/", $file->getClientOriginalName());
				$succeeded[] = $file->getClientOriginalName();
			} else {
				$failed[] = $file->getClientOriginalName();
			}
		}
		$return = ['succeeded'=>$succeeded,'failed'=>$failed];
		echo $return['succeeded'][0];
		$page_content = $templating->render('upload_image_success.php',$return);
		$templating->setTitle('Upload Image');
		$templating->addGlobal('page_content', $page_content);

		return $templating->renderDefault();
	}
	public function uploadView(Request $request, Application $app) {
		$templating = new WebTemplate();

		$page_content = $templating->render('upload_image.php');
		
		$templating->setTitle('Upload Image');
		$templating->addGlobal('page_content', $page_content);

		return $templating->renderDefault();
	}
}
?>
