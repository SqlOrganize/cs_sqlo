


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
Entity alumno = db.entity("alumno");
Console.WriteLine(alumno._nf.Count);
for (int i = 0; i < alumno._nf.Count; i++)
{
    Console.WriteLine(alumno._nf[i]);
}

