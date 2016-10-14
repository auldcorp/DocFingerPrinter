<?php
namespace Napkins;

require_once(__DIR__ . '/class_template.php');

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;

Class UploadController {
	public function uploadAction(Request $request, Application $app) {
		$succeeded = False;
		$data = $request->files->get('form');
		$succeeded = [];
		$failed = [];
		foreach($data['files'] as $file) {
			if($file == Null) {
				return $this->uploadView($request, $app);
			}
			if(preg_match("/^image\/[a-zA-Z|\-]{2,10}/",$file->getMimeType())) {
				$file->move("/srv/napkin/images/", $file->getClientOriginalName());
				array_push($succeeded, $file->getClientOriginalName());
			} else {
				array_push($failed, $file->getClientOriginalName());
			}
		}
		$return = ['succeeded'=>$succeeded,'failed'=>$failed];

		return $this->uploadView($request, $app, $return);
	}
	public function uploadView(Request $request, Application $app, Array $var_content = []) {
		$templating = new WebTemplate();

		$page_content = $templating->render('upload_image.php', $var_content);
		
		$templating->setTitle('Upload Image');
		$templating->addGlobal('page_content', $page_content);

		return $templating->renderDefault();
	}
}
?>
