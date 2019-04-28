using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkdayApi;
using SoapInterface = WorkdayApi.WorkDayService.StaffingPort;
using SoapImpl = WorkdayApi.WorkDayService.StaffingPortClient;
using Moq;
using WorkdayApi.WorkDayService;
using System.Linq;
using System.Configuration;
using System.ServiceModel;

namespace WorkDayApiE2e
{
  [TestClass]
  public class WorkDayApiTest
  {
    [TestMethod]
    public void InstantiatesWithoutError()
    {
      var api = new EmployerDemography();

      Assert.IsInstanceOfType(api, typeof(EmployerDemography));
    }

    [TestMethod]
    public void CallsSoapMethodGetWorkers()
    {
      var isMockMethodCalled = false;

      var mock = new Mock<SoapInterface>();
      mock.Setup(a => a.Get_Workers(It.IsAny<Get_WorkersInput>()))
        .Callback(() => isMockMethodCalled = true);
      var mockSoap = mock.Object;

      var api = new EmployerDemography(mockSoap);
      api.GetWorkerById("1");

      Assert.IsTrue(isMockMethodCalled);
    }

    [TestMethod]
    public void ReturnsCorrectResponse()
    {
      var soapEndpointWithNoResponse = ConfigurationManager.AppSettings["SoapEndpointWithNoData"];
      var api = new EmployerDemography();

      var response = api.GetWorkerById("1");

      Assert.IsInstanceOfType(response, typeof(Worker_DataType));
      Assert.AreEqual(response.User_ID, "john.doe");
      Assert.AreEqual(response.Worker_ID, "100727");
      Assert.AreEqual(
        response
        .Personal_Data
        .Name_Data
        .Legal_Name_Data
        .Name_Detail_Data
        .Country_Reference
        .Descriptor,
        "United States of America"
        );
      Assert.AreEqual(
        response
        .Personal_Data
        .Contact_Data
        .Address_Data[0]
        .Address_Line_Data[0]
        .Value,
        "3524 E. 24th Ave"
        );
      Assert.AreEqual(
        response
        .Personal_Data
        .Contact_Data
        .Address_Data[1]
        .Address_Line_Data[0]
        .Value,
        "11016 E Main Drive"
        );

      /// Here we are changing endpoint to one that responds with zero body
      /// Thero body cannot be parsed by xml, so that will throw exception
      var soapDefaultImpl = new SoapImpl();
      soapDefaultImpl.Endpoint.Address
        = new System.ServiceModel.EndpointAddress(soapEndpointWithNoResponse);
      api = new EmployerDemography(soapDefaultImpl);

      Assert.ThrowsException<CommunicationException>(() => api.GetWorkerById("1"));
    }
  }
}
