


using cs_sqlo;
using cs_sqlo_ss;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data.Common;
using System.Data;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

Dictionary<string, object> config = new Dictionary<string, object>()
 {
    //{ "connection_string", "server=localhost;uid=root;pwd=;database=planfi10_20203"},
    //{ "path_model","C:\\projects\\cs_sqlo\\ConsoleApp1\\model\\"},
    { "connection_string", "Data Source=DQFC2G3;Initial Catalog=Gestadm_CTAPilar;Integrated Security=True;TrustServerCertificate=true;"},
    { "path_model","C:\\projects\\cs_sqlo\\ConsoleApp1\\model\\"}, //localhost
};

//var db = new DbMy(config);
DbSs db = new(config);
var data = db.query("SUJETOS").
    Where("$FECHA_CARGA = @0").
    Parameters("2007-12-04").
    fetch_all();


string json = JsonConvert.SerializeObject(data, Formatting.Indented);

Console.WriteLine(json);





