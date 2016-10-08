<?php

namespace Napkins;


Class LoginController
{
private $form;

	public function defaultAction($action, Request $request, Application $app )
	{
		$this->form = new forms;
		
		if($action == 'login_view')
		{
			return $this->loginView();
		}
		else if($action == 'login')
		{
			return $this->loginAction($request, $app);
		}
		else if($action == 'logout')
		{
			return $this->logoutAction();
		}
		else if($action == 'register')
		{
			return $this->registerView();
		}
		else if($action == 'register_user')
		{
			return $this->registerUser($request, $app);
		}
		else
		{
			var_dump($action);
			return new Response('Routing Error!');
		}
	}

	public function loginView(){
		$template = new WebTemplate();
		$pageContent = $template->render('login.php', ['form' => $this->form]);

		$template->setTitle('Login');
		$template->addGlobal('page_content', $page_content);

		return $template->renderDefault();
	}

	public function registerView(Request $request, Application $app)
	{
		$templating = new WebTemplate();

		$forms = new forms('register', FALSE, FALSE);

		$page_content = $templating->render('register.php', array('form' => &$form));

		$templating->setTitle('Register');
		$templating->addGlobal('page_content', $page_content);

		return $templating->renderDefault();
	}

	private function registerUser(Request $request, Application $app)
	{
		$Errors = [];
		$data = $request->request->all();

		if(!isset($data['password'])) {
			array_push($Errors, 'Password must be entered');
		} else {
			$password_given = $data['password'];
		}

		if(!isset($data['email'])) {	
			array_push($Errors, 'Email address must be entered');
		} else {
			$email = mb_strtolower($data['email']);
		}

		if(!isset($data['full_name'])) {
			array_push($Errors, 'Full Name must be entered');
		} else {
			$full_name = $data['full_name'];
		}
		
		if(empty($Errors))
		{
			$sql = "INSERT INTO users (email, password, name) VALUES( :email, :password, :full_name)";
			
			$stmt = $app['db']->prepare($sql);
			$stmt->bindValue('email', $email);
			$stmt->bindValue('password', password_hash($password_given, PASSWORD_DEFAULT));
			$stmt->bindValue('full_name',$full_name);

			$stmt->execute();

			return new Response('Success!');
		} else {
			$templating = new WebTemplate();

			unset($data['password']);

			$page_content = $templating->render('register.php', array('form_data' => $data));

			$templating->setTitle('Register');
			$templating->addGlobal('page_content', $page_content);

			return $templating->renderDefault($Errors);
		}

	}

	private function loginAction(Request $request, Application $app)
	{
		$data = $request->request->all();

		if (empty($data['password']) || empty($data['email'])) {
			$this->form->error('email', 'Please enter an email and password');
		} else {
			$password_given = $data['password'];

			$email = mb_strtolower($data['email']);

			$stmt = $app['db']->query('SELECT password FROM users WHERE email=' . $app['db']->quote($email) . ' LIMIT 1');
			$stmt = $stmt->fetch();
			$password_hash = $stmt['password'];
			var_dump($password_given, $password_hash, $email);
			if( $password_hash === FALSE || ! password_verify($password_given, $password_hash))
			{
				$this->form->error('password', 'The email or password was incorrect. Please try again.');
			}
		}
				
		$this->form->validation_done();


		if ($this->form->is_ready()) {
			$stmt = $app['db']->query('SELECT id FROM users WHERE email=' . $app['db']->quote($email) . ' LIMIT 1');
			$stmt = $stmt->fetch();
			$user_id = $stmt['id'];

			$sess = $app['session'];
			$sess->set('user_id', $user_id);
			$sess->set('email', $email);
			
			return new Response('Success!');
		} else {
			return $this->loginView($request, $app);
		}
	}

	private function logoutAction(Application $app)
	{
		$templating = new WebTemplate();
		
		$sess = $app['session'];
		$sess->invalidate();

		return $app->redirect('/login');
	}

}

?>
