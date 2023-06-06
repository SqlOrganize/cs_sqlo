


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
string query = db.query("alumno")
    .fa("$persona-nombres.max")
    .w("$persona-apellidos IS NOT NULL")
    .g("$persona_nombres, $persona_apellidos").sql();
//.w("($persona-nombres.count = @p1)")



//Entity alumno = db.entity("alumno");


