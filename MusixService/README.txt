class Service {
	int foo(int a, int b);
	int bar();
}

JsonRequest
	method
	id
	params


class Request {
	getMethode
	getResource
	getProtocol
	getHeader(name)
	getParam(name)
	
	parse(stream)
}

class Response {
	setStatusCode
	setStatusText
	setHeader();
	redirect(url);
	
	beginObject();
	beginArray();
	begin
}

class Session {
	start(id)
	destroy()
	abandon()
	set(key, value)
	get(key)
}

class Context {
	Application

	Config

	Cookies

	Request
	
	Response

	Session

	Data
}

class Controller {
	
	
	void dispatch()
	
}

class HomeController {

	indexAction(Context c) {
	}

}

class Application {

	void addRoute(url, controllerName, actionName)
	
	void createRequest()

	void createResponse()

	void createCookies()

	void createSession()

	void serve()	
}

class Program {
	public static void main() {
		Application.addRoute("/test/", HomeController.class, "index");
		Application.serve();
	}
}















class JsonRpcService {
	register("foo", foo);
}