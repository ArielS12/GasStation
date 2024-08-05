using GasStation.Main;
using GasStation.Model;

Console.WriteLine("Ingrese la cantidad de surtidores");
int s;
var pumpNumber = int.TryParse(Console.ReadLine(), out s);

var pumps = new List<Pump>();
for (int p = 0; p < s; p++)
{
    pumps.Add(new Pump
    {
        Id = p,
    });
}


while (!pumpNumber)
{
    Console.WriteLine("Debe ingresar un número");
    pumpNumber = int.TryParse(Console.ReadLine(), out s);
}

int a = 0;
var client = new StationClient(pumps);
var accion = true;

while (a != 8)
{ 
    Console.WriteLine("Que acción desea realizar: \n" +
        "1. Liberar surtidor \n" +
        "2. Bloquear surtidor \n" +
        "3. Prefijar importe \n" +
        "4. Obtener estado de los sutidores \n" +
        "5. Comenzar a cargar \n" +
        "6. Detener la carga \n" +
        "7. Obtener historial \n" +
        "8. Salir");

    accion = int.TryParse(Console.ReadLine(), out a);

    while (!accion || a <= 0 || a >= 9)
    {
        Console.WriteLine("Debe ingresar una acción valida");
        accion = int.TryParse(Console.ReadLine(), out a);
    }

    switch (a) {
        case 1:
            PumpAction(PUMP_ACTION.RELEASE);
            break;
        case 2:
            PumpAction(PUMP_ACTION.BLOCK);
            break;
        case 3:
            SetPredefinedAmount();
            break;
        case 4:
            ShowPumpStatus();
            break;
        case 5:
            PumpAction(PUMP_ACTION.START_CHARGING);
            break;
        case 6:
            PumpAction(PUMP_ACTION.STOP_CHARGING);
            break;
        case 7:
            GetHistorical();
            break;
        case 8:
            break;
    }
    
    if(a != 8)
        NewAction();

}

void GetHistorical() {
    foreach (var log in client.GetLog())
    {
        Console.WriteLine($"Numero de surtidor: {log.PumpId}, Monto cargado: {log.Amount} euros, Fecha y hora de la carga: {log.CreatedDate}");
    }
}

void SetPredefinedAmount() {
    int p;
    string response;

    Console.WriteLine("¿Que surtidor va a utilizar?");
    var pumpIsValid = int.TryParse(Console.ReadLine(), out p);

    while (!pumpIsValid || p <= 0 || p >= pumps.Count())
    {
        Console.WriteLine("El número de surtidor no existe. Ingrese un número de surtidor valido");
        accion = int.TryParse(Console.ReadLine(), out p);
    }

    int m;
    Console.WriteLine("Ingrese el monto predefinido");
    var predefinedAmount = int.TryParse(Console.ReadLine(), out m);

    while (!predefinedAmount || m < 0)
    {
        Console.WriteLine("El monto ingresado no es valido, ingrese un monto mayor a 0");
        accion = int.TryParse(Console.ReadLine(), out m);
    }

    
    var actionResult = client.SetPredefinedAmount(p, m);
    if (actionResult.Success)
        response = "El precio se ha establecido correctamente";
    else response = actionResult.ErrorMessage;
    Console.WriteLine(response);
}

void NewAction() {
    Console.WriteLine("¿Desea realizar otra acction?: Si/No");
    var response = Console.ReadLine();

    while (response != "Si" && response != "No")
    {
        Console.WriteLine("Debe response Si o No");
        response = Console.ReadLine();
    }

    a = response == "Si" ? 0 : 8;
}

void ShowPumpStatus()
{
    foreach (var status in client.GetPumpsStates())
    {
        Console.WriteLine(status);
    }
}

void PumpAction(PUMP_ACTION action) {
    string response;
    int p;
    Console.WriteLine(ActionToString(action));
    var pumpIsValid = int.TryParse(Console.ReadLine(), out p);

    while (!pumpIsValid || p <= 0 || p >= pumps.Count())
    {
        Console.WriteLine("El número de surtidor no existe. Ingrese un número de surtidor valido");
        accion = int.TryParse(Console.ReadLine(), out p);
    }

    var actionResult = client.DoPumpAction(p, action);
    if (actionResult.Success)
        response = ActionResponseToString(action);
    else response = actionResult.ErrorMessage;
    Console.WriteLine(response);
}


string ActionToString(PUMP_ACTION action) {
    string stringAction = "";

    switch (action) { 
        case PUMP_ACTION.RELEASE:
            stringAction = "¿Que surtidor desea liberar?";
            break;
        case PUMP_ACTION.BLOCK:
            stringAction = "¿Que surtidor desea bloquear?";
            break;
        case PUMP_ACTION.START_CHARGING:
            stringAction = "¿Que surtidor desea comenzar a utilizar?";
            break;
        case PUMP_ACTION.STOP_CHARGING:
            stringAction = "¿Que surtidor desea detener?";
            break;
    }

    return stringAction;
}

string ActionResponseToString(PUMP_ACTION action)
{
    string stringAction = "";

    switch (action)
    {
        case PUMP_ACTION.RELEASE:
            stringAction = "El surtidor se ha liberado exitosamente";
            break;
        case PUMP_ACTION.BLOCK:
            stringAction = "El surtidor se ha bloqueado exitosamente";
            break;
        case PUMP_ACTION.START_CHARGING:
            stringAction = "El surtidor ha comenzado a cargar";
            break;
        case PUMP_ACTION.STOP_CHARGING:
            stringAction = "El surtidor ha dejado de cargar";
            break;
    }

    return stringAction;
}