
using cs_sqlo_my;

Dictionary<string, object> config = new Dictionary<string, object>()
 {
    { "connection_string", "server=localhost;uid=root;pwd=;database=planfi10_20203"},
    { "path_model","C:\\xampp\\htdocs\\cs_sqlo\\ConsoleApp1\\model\\"},
};

var db = new DbMy(config);
Console.WriteLine(db.ToString());

