using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkdayApi.WorkDayService;
using SoapInterface = WorkdayApi.WorkDayService.StaffingPort;
using SoapImpl = WorkdayApi.WorkDayService.StaffingPortClient;
using Worker = WorkdayApi.WorkDayService.Worker_DataType;

namespace WorkdayApi
{
  public class EmployerDemography
  {
    protected readonly SoapInterface soap;
    public EmployerDemography(SoapInterface soap = null)
    {
      if (soap == null)
        soap = new SoapImpl();

      this.soap = soap;
    }

    public Worker GetWorkerById(string id)
    {
      var request = PrepareRequest(id);

      return soap.Get_Workers(request)
        ?.Get_Workers_Response
        ?.Response_Data
        ?.First()
        ?.Worker_Data;
    }

    virtual protected Get_WorkersInput PrepareRequest(string id)
    {
      var request = new Get_WorkersInput(
        null,
        new Get_Workers_RequestType()
        {
          Request_References = new Worker_Request_ReferencesType()
          {
            Worker_Reference = new[] {
              new WorkerObjectType()
              {
                ID = new [] {
                  new WorkerObjectIDType(){type="Employee_ID",Value="100727"
                  }
                }
              }
            }
          }
        }
        );

      return request;
    }
  }
}
