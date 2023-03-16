using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using RestSharp;
using System.Net;
using System.Text.Json;

namespace RestSharpAPITests
{
    public class RestSharpAPI_Tests
    {
        private RestClient client;
        private const string baseUrl = "https://contactbook.dboteva.repl.co/api";

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(baseUrl);
        }

        [Test]
        public void Test_ListContacts_CheckFirstContactFirstAndLastName()
        {
            var request = new RestRequest("contacts", Method.Get);
            var response = this.client.Execute(request);

            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contact = JsonSerializer.Deserialize<List<Contact>>(response.Content);

            Assert.That(contact[0].firstName, Is.EqualTo("Steve"));
            Assert.That(contact[0].lastName, Is.EqualTo("Jobs"));
        }

        [Test]
        public void Test_FindContactByKeyword_CheckFirstContactFirstAndLastName()
        {
            var request = new RestRequest("contacts/search/albert ", Method.Get);
            var response = this.client.Execute(request);

            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contact = JsonSerializer.Deserialize<List<Contact>>(response.Content);

            Assert.That(contact[0].firstName, Is.EqualTo("Albert"));
            Assert.That(contact[0].lastName, Is.EqualTo("Einstein"));
        }

        [Test]
        public void Test_FindContactByKeyword_MissingRandomNumber_EmptyResult()
        {
            var request = new RestRequest("contacts/search/missing6787", Method.Get);
            var response = this.client.Execute(request);

            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contact = JsonSerializer.Deserialize<List<Contact>>(response.Content);

            Assert.That(response.Content, Is.EqualTo("[]"));
        }

        [Test]
        public void Test_CreateContactInvalidData()
        {
            var request = new RestRequest("contacts", Method.Post);
            var body = new
            {
                lastName = "Curie",
                email= "marie67@gmail.com",
                phone = "+1 800 200 300",
                comments = "Old friend"
            };

            request.AddBody(body);
            var response = this.client.Execute(request);
            
            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"First name cannot be empty!\"}"));
        }

        [Test]
        public void Test_CreateContactValidData()
        {
            var request = new RestRequest("contacts", Method.Post);
            var body = new
            {
                firstName = "Marie",
                lastName = "Curie",
                email = "marie67@gmail.com",
                phone = "+1 800 200 300",
                comments = "Old friend"
            };

            request.AddBody(body);
            var response = this.client.Execute(request);
            var contactObject = JsonSerializer.Deserialize<contactObject>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(contactObject.msg, Is.EqualTo("Contact added."));
            Assert.That(contactObject.contact.id, Is.GreaterThan(0));
            Assert.That(contactObject.contact.firstName, Is.EqualTo(body.firstName));
            Assert.That(contactObject.contact.lastName, Is.EqualTo(body.lastName));
            Assert.That(contactObject.contact.email, Is.EqualTo(body.email));
            Assert.That(contactObject.contact.phone, Is.EqualTo(body.phone));
            Assert.That(contactObject.contact.dateCreated, Is.Not.Empty);
            Assert.That(contactObject.contact.comments, Is.Not.Empty);
        }
    }
}