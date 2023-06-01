


using cs_sqlo;
using cs_sqlo_ss;
using System.ComponentModel;


Dictionary<string, object> config = new Dictionary<string, object>()
 {
    //{ "connection_string", "server=localhost;uid=root;pwd=;database=planfi10_20203"},
    //{ "path_model","C:\\projects\\cs_sqlo\\ConsoleApp1\\model\\"},
    { "connection_string", "Data Source=DQFC2G3;Initial Catalog=Gestadm_CTAPilar;Integrated Security=True;TrustServerCertificate=true;"},
    { "path_model","C:\\projects\\cs_sqlo\\ConsoleApp1\\model\\"}, //localhost
};

//var db = new DbMy(config);
DbSs db = new(config);

var field_names = db.field_names("alumno");
var field_names_r = db.tools("alumno").field_names();

var query = db.query("alumno").size(100).page(2).cond(new List<object>() {
        new List<object>(){"field", "EQUAL", "something"},
        new List<object>() { "field2", "GREATER", "something2" }
})
.cond("field5", "EQUAL", "something5", "OR")
.param("field3", "something3")
.order(new Dictionary<string, string>()
{
    { "field1", "ASC" },
    { "field2", "DESC" }
})
.order("field3")
.order("field4", "DESC");


Console.WriteLine(query.ToString());

//Entity alumno = db.entity("alumno");


