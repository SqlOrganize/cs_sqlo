


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

var MyText = "($field_id = $something) and ($field_id = hola)";

var traduce = "";
int field_start = 0;

for (int i = 0; i < MyText.Length; i++)
{
    Console.WriteLine(MyText[i]);

    if (MyText[i] == '$')
    {
        field_start = i;
        continue;
    }

    if (field_start != 0)
    {
        if ((MyText[i] != ' ') && (MyText[i] != ')')) continue;
        var f = MyText.Substring(field_start+1, i-field_start-1);
        Console.WriteLine(f);
        var convert = "persona.nombres AS 'persona-nombres'";
        traduce += convert;
        field_start = 0;
    }

    traduce += MyText[i];

}

Console.WriteLine(traduce);


//var db = new DbMy(config);
//DbMy db = new(config);
//db.query("alumno").cond(new List<object> { "anio_ingreso","equal","1"}).build();

//db.query("alumno").cond("$persona-nombres = $persona")
//System.Console.WriteLine("end");
//Entity alumno = db.entity("alumno");
//Console.WriteLine(alumno.nf.Count);
//for (int i = 0; i < alumno.nf.Count; i++)
//{
//    Console.WriteLine(alumno.nf[i]);
//}

//(("anio_ingreso.max -APPROX- 1) AND (anio_ingreso.max = null))";