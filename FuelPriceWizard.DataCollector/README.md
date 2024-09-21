# FuelPriceWizard - DataCollector
This sub-project serves as the base project for data collection services. The actual services can be added via an appsettings.json entry and placement of the .dll file in the project folder.

## Create a new data collection service
You can create a class library with a service class, specific appsettings and utilize DI (DependencyInjection)

### 1. Create a new ClassLibrary in this solution
### 2. Add an ``appsettings.json`` file naming it ``appsettings.<ServiceClassName>.json``
In the appsettings file you have to define fetch settings as well as any service specific settings.
The FetchSettings section is defined as follows:

```json
"FetchSettings": {
  "ExcludedWeekdays": [ "Saturday", "Sunday" ],
  "IntervalValue": 5,
  "IntervalUnit": "Second",
  "StartNextFullHour": false
}
```

| Name | Description | Possible Values |
| :- | :-:         | :-    |
| ExcludedWeekdays | Defines a list of weekdays where the service is not run or where the service is skipped | <ul><li>Monday</li><li>Tuesday</li><li>Wednesday</li><li>Thursday</li><li>Friday</li><li>Saturday</li><li>Sunday</li></ul> |
| IntervalValue | Defines the interval in which the periodic service is run | any number value |
| IntervalUnit | Defines the interval unit of the interval value (every x units) | <ul><li>Second (every x seconds)</li><li>Minute (every x minutes)</li><li>Hour (every x hours)</li></ul> |
| StartNextFullHour | Defines if the collection circle is run with the interval starting on the next full hour or immediatly with the start of the service<br>``(e.g. service is started at 12:43 but the first collection circle starts at 13:00)`` | <ul><li>true</li><li>false</li></ul> |

### 3. Create the entrypoint class implementing the ``IFuelPriceSourceService`` interface included in the ``FuelPriceWizard.BusinessLogic`` project

To learn more about the ``IFuelPriceSourceService`` interface click [here](../FuelPriceWizard.BusinessLogic/IFuelPriceSourceService.cs).

## Adding a new service
To add a new service create a new project and add a new entry in the appsettings.json file under the **ImplementationAssemblies** section:

```json
"ImplementationAssemblies": [
  {
    "FilePath": "MockUpFuelPriceSourceImplementation.dll",
    "Type": "MockUpFuelPriceSourceImplementation.MockUpFuelPriceService"
  }
  ...
]
```

- ***FilePath:*** Defines ***where the .dll file is located***, normally this is the active folder (working directory) and therefore the filename itsself is sufficient but if the file is located somewhere else you would have to specify the relative path here. (``e.g. ..\\..\\..\\..\\MockUpFuelPriceSourceImplementation\\bin\\debug\\net8.0\\MockUpFuelPriceSourceImplementation.dll`` => Specifies the .dll file in the build output directory of the MockUpFuelPriceSourceImplementation project)
- ***Type:*** Defines the ***full name of the service class*** (including its namespace)
