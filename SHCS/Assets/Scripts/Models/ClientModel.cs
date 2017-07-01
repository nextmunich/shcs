using UnityEngine;
using System.Collections;

public class ClientModel
{
    public string Id { get; set; }

    public string Address1 { get; set; }

    public string Address2 { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PhoneNumber { get; set; }

    public string PostalCode { get; set; }

    public override string ToString()
    {
        return string.Format("[ClientModel: Id={0}, Address1={1}, Address2={2}, City={3}, Country={4}, FirstName={5}, LastName={6}, PhoneNumber={7}, PostalCode={8}]", Id, Address1, Address2, City, Country, FirstName, LastName, PhoneNumber, PostalCode);
    }
}