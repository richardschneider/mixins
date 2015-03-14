# Welcome to Mixins

Creates an instance that is a composite of multiple objects and/or properties.  Basically a poor man's multiple inheritence that is aimed at serialisation and deserialisation of object graphs. See [Wikipedia mixins](http://en.wikipedia.org/wiki/Mixin) for more details.

## Story

* **As a** Web API developer
* **I want** to easily create composite objects
* **So that** I can avoid writing tedeaoous codes that maps/converts multiple objects into one.
In particular, I want to combine multiple objects and/or properties from my data model into one object that is serialised to my clients.

## Use case

My service layer provides a `Contact` and a `Address` object, but for the API consumer I want the address to be included into contact that I return to the consumer.  I also want to add the "_self" property, which is the URL to the contact. For example

```C#
[Route("contact/{id:int}"]
public Mixin Read(int id)
{
  return new Mixin
     .With("_self", BaseUrl + "contact/" + id)
     .With(service.GetContact(id))
     .With(service.GetContactAddress(id));
}

[Route("contact"), HttpPost]
public string Create(Mixin mixin)
{
  var id = service.CreateContact(mixin.As<Contact>());
  var address = mixin.As<Address>();
  address.ContactId = id;
  service.CreateContactAddress(address);

  return id;
}
```
