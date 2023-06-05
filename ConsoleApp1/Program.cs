


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


string var = @"[
    ['field', 'equal', 'something'],
    ['field2', 'equal', 'something'],
    [
        ['field3', 'equal', 'something'],
        ['field3', 'equal', 'something', 'OR']  
    ]
]
";
var query2 = db.query("planificacion").unique(("plan","31073351"),("anio","20310733513")).cond(@"
sda
");

Console.WriteLine(query2.ToString());

//Entity alumno = db.entity("alumno");


