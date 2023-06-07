


using cs_sqlo;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

Dictionary<string, object> config = new Dictionary<string, object>()
 {
    { "connection_string", "server=localhost;uid=root;pwd=;database=planfi10_20203"},
    { "path_model","C:\\xampp\\htdocs\\cs_sqlo\\ConsolaMy\\model\\"},
    
    //{ "connection_string", "Data Source=DQFC2G3;Initial Catalog=Gestadm_CTAPilar;Integrated Security=True;TrustServerCertificate=true;"},
    //{ "path_model","C:\\projects\\cs_sqlo\\ConsoleApp1\\model\\"}, //localhost
};

DbMy db = new(config);
string query = db.query("alumno")
    .FieldsAs("$persona-nombres.max")
    .Where("$persona-apellidos IS NOT NULL")
    .Group("$persona_nombres, $persona_apellidos")
    .Order("$persona_nombres ASC, $persona_apellidos DESC")
    .Sql();

Console.WriteLine(query);
