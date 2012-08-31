using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrissCrossLib.ReportWebService;
using CrissCrossLib;
using Rhino.Mocks;

namespace CrissCrossTests.TestDoubles
{
    /// <summary>
    /// Various tests need a mock web service to talk to.
    /// This class can be used to make mock SSRS web service clients
    /// </summary>
    public class MockSsrsWebServiceFactory
    {

        public CrcSsrsSoapClientFactory MakeMockSoapClientFactory(ReportingService2005Soap serviceClient)
        {
            // mock the factory
            var soapClientFactory = MockRepository.GenerateMock<CrcSsrsSoapClientFactory>();
            soapClientFactory.Expect(s => s.MakeSsrsSoapClient()).Return(serviceClient);
            return soapClientFactory;
        }


        public ReportingService2005Soap MakeMockReportingService2005Soap(ReportParameter[] paramsToReturn)
        {
            // mock the web service
            var soapClientMock = MockRepository.GenerateStub<ReportingService2005Soap>();

            var grpResponse = new GetReportParametersResponse(null, paramsToReturn);
            soapClientMock.Expect(s => s.GetReportParameters(null)).IgnoreArguments()
                .Return(grpResponse);
            
            return soapClientMock;
        }

        public ReportingService2005Soap MakeMockReportingService2005Soap(string singleSpecificValue, ReportParameter[] paramsToReturn)
        {
            var soapClientMock = MockRepository.GenerateStub<ReportingService2005Soap>();

            var grpResponse = new GetReportParametersResponse(null, paramsToReturn);
            soapClientMock.Expect(s => s.GetReportParameters(
                Arg<GetReportParametersRequest>.Matches(
                grp =>  grp.Values.Count() == 1 && grp.Values[0].Value == singleSpecificValue)
                )).Return(grpResponse);

            return soapClientMock;

        }

        public void SetListChildrenExpectation(ReportingService2005Soap mockService, string path, CatalogItem[] returnItems)
        {
            var lcResponse = new ListChildrenResponse(null, returnItems);
            mockService.Expect(m => m.ListChildren(
                Arg<ListChildrenRequest>.Matches(
                lcr => lcr.Item == path)
                )).Return(lcResponse);
        }

        public ReportParameter[] MakeSimpleTestParameters()
        {
            
            ReportParameter p1 = new ReportParameter();
            p1.Name = "ParamOne";
            p1.ValidValues = new ValidValue[]{ new ValidValue(){Label = "Label1", Value = "Value1"},
                                    new ValidValue(){Label = "Label2", Value = "Value2"}};
            p1.DefaultValues = new string[] { };
            ReportParameter p2 = new ReportParameter();
            p2.Name = "ParamTwo";
            p2.ValidValues = new ValidValue[]{ new ValidValue(){Label = "Label3", Value = "Value3"},
                                    new ValidValue(){Label = "Label4", Value = "Value4"}};
            p2.DefaultValues = new string[] { };

            ReportParameter[] paramArray = new ReportParameter[] { p1, p2 };
            return paramArray;
        }

        public ReportParameter[] MakeDateTestParameters()
        {

            ReportParameter p1 = new ReportParameter();
            p1.Name = "ParamOne";
            p1.Type = ParameterTypeEnum.DateTime;
            p1.DefaultValues = new string[] { };
            ReportParameter p2 = new ReportParameter();
            p2.Name = "ParamTwo";
            p2.Type = ParameterTypeEnum.DateTime;
            p2.DefaultValues = new string[] { };

            ReportParameter[] paramArray = new ReportParameter[] { p1, p2 };
            return paramArray;
        }

        public ReportParameter[] MakeDependantTestParameters()
        {

            ReportParameter p1 = new ReportParameter();
            p1.Name = "ParamOne";
            p1.ValidValues = new ValidValue[]{ new ValidValue(){Label = "Label1", Value = "Value1"},
                                    new ValidValue(){Label = "Label2", Value = "Value2"}};
            p1.DefaultValues = new string[] { };
            ReportParameter p2 = new ReportParameter();
            p2.Name = "ParamTwo";
            p2.ValidValues = new ValidValue[]{ new ValidValue(){Label = "Label3", Value = "Value3"},
                                    new ValidValue(){Label = "Label4", Value = "Value4"}};
            p2.DefaultValues = new string[] { };
            p2.Dependencies = new string[] { "ParamOne" };
            ReportParameter p3 = new ReportParameter();
            p3.Name = "ParamThree";
            p3.ValidValues = new ValidValue[]{ new ValidValue(){Label = "Label5", Value = "Value5"},
                                    new ValidValue(){Label = "Label6", Value = "Value6"}};
            p3.DefaultValues = new string[] { };
            p3.Dependencies = new string[] { "ParamOne" };


            ReportParameter[] paramArray = new ReportParameter[] { p1, p2, p3 };
            return paramArray;
        }

        


    }
}
