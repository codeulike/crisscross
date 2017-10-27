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

        public CrcSsrsSoapClientFactory MakeMockSoapClientFactory(ReportingService2010Soap serviceClient)
        {
            // mock the factory
            var soapClientFactory = MockRepository.GenerateMock<CrcSsrsSoapClientFactory>();
            soapClientFactory.Expect(s => s.MakeSsrsSoapClient()).Return(serviceClient);
            return soapClientFactory;
        }


        public ReportingService2010Soap MakeMockReportingService2010Soap(ItemParameter[] paramsToReturn)
        {
            // mock the web service
            var soapClientMock = MockRepository.GenerateStub<ReportingService2010Soap>();

            var grpResponse = new GetItemParametersResponse(null, paramsToReturn);
            soapClientMock.Expect(s => s.GetItemParameters(null)).IgnoreArguments()
                .Return(grpResponse);
            
            return soapClientMock;
        }

        public ReportingService2010Soap MakeMockReportingService2010Soap(string singleSpecificValue, ItemParameter[] paramsToReturn)
        {
            var soapClientMock = MockRepository.GenerateStub<ReportingService2010Soap>();

            var grpResponse = new GetItemParametersResponse(null, paramsToReturn);
            soapClientMock.Expect(s => s.GetItemParameters(
                Arg<GetItemParametersRequest>.Matches(
                grp =>  grp.Values.Count() == 1 && grp.Values[0].Value == singleSpecificValue)
                )).Return(grpResponse);

            return soapClientMock;

        }

        public void SetListChildrenExpectation(ReportingService2010Soap mockService, string path, CatalogItem[] returnItems)
        {
            var lcResponse = new ListChildrenResponse(null, returnItems);
            mockService.Expect(m => m.ListChildren(
                Arg<ListChildrenRequest>.Matches(
                lcr => lcr.ItemPath == path)
                )).Return(lcResponse);
        }

        public ItemParameter[] MakeSimpleTestParameters()
        {
            
            ItemParameter p1 = new ItemParameter();
            p1.Name = "ParamOne";
            p1.ValidValues = new ValidValue[]{ new ValidValue(){Label = "Label1", Value = "Value1"},
                                    new ValidValue(){Label = "Label2", Value = "Value2"}};
            p1.DefaultValues = new string[] { };
            ItemParameter p2 = new ItemParameter();
            p2.Name = "ParamTwo";
            p2.ValidValues = new ValidValue[]{ new ValidValue(){Label = "Label3", Value = "Value3"},
                                    new ValidValue(){Label = "Label4", Value = "Value4"}};
            p2.DefaultValues = new string[] { };

            ItemParameter[] paramArray = new ItemParameter[] { p1, p2 };
            return paramArray;
        }

        public ItemParameter[] MakeDateTestParameters()
        {

            ItemParameter p1 = new ItemParameter();
            p1.Name = "ParamOne";
            p1.ParameterTypeName = CrcReportDefinitionFactory.ReportServiceParameterTypes.DateTime;
            p1.DefaultValues = new string[] { };
            ItemParameter p2 = new ItemParameter();
            p2.Name = "ParamTwo";
            p2.ParameterTypeName = CrcReportDefinitionFactory.ReportServiceParameterTypes.DateTime; 
            p2.DefaultValues = new string[] { };

            ItemParameter[] paramArray = new ItemParameter[] { p1, p2 };
            return paramArray;
        }

        public ItemParameter[] MakeDependantTestParameters()
        {

            ItemParameter p1 = new ItemParameter();
            p1.Name = "ParamOne";
            p1.ValidValues = new ValidValue[]{ new ValidValue(){Label = "Label1", Value = "Value1"},
                                    new ValidValue(){Label = "Label2", Value = "Value2"}};
            p1.DefaultValues = new string[] { };
            ItemParameter p2 = new ItemParameter();
            p2.Name = "ParamTwo";
            p2.ValidValues = new ValidValue[]{ new ValidValue(){Label = "Label3", Value = "Value3"},
                                    new ValidValue(){Label = "Label4", Value = "Value4"}};
            p2.DefaultValues = new string[] { };
            p2.Dependencies = new string[] { "ParamOne" };
            ItemParameter p3 = new ItemParameter();
            p3.Name = "ParamThree";
            p3.ValidValues = new ValidValue[]{ new ValidValue(){Label = "Label5", Value = "Value5"},
                                    new ValidValue(){Label = "Label6", Value = "Value6"}};
            p3.DefaultValues = new string[] { };
            p3.Dependencies = new string[] { "ParamOne" };


            ItemParameter[] paramArray = new ItemParameter[] { p1, p2, p3 };
            return paramArray;
        }

        


    }
}
