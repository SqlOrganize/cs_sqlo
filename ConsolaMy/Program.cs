


using cs_sqlo;
using cs_sqlo_my;
using System.ComponentModel;

Dictionary<string, object> config = new Dictionary<string, object>()
 {
    { "connection_string", "server=localhost;uid=root;pwd=;database=planfi10_20203"},
    { "path_model","C:\\xampp\\htdocs\\cs_sqlo\\ConsolaMy\\model\\"},
    
    //{ "connection_string", "Data Source=DQFC2G3;Initial Catalog=Gestadm_CTAPilar;Integrated Security=True;TrustServerCertificate=true;"},
    //{ "path_model","C:\\projects\\cs_sqlo\\ConsoleApp1\\model\\"}, //localhost
};

//var db = new DbMy(config);
DbMy db = new(config);
Entity alumno = db.entity("alumno");
Console.WriteLine(alumno.nf.Count);
for (int i = 0; i < alumno.nf.Count; i++)
{
    Console.WriteLine(alumno.nf[i]);
}

