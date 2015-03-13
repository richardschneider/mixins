# mixins

* **As a** Web API developer
* **I want** to easily create composite objects
* **So that** I can avoid writing tedeaoous codes that maps/converts multiple objects into one.
In particular, I want to combine multiple objects and/or properties from my data model into one object that is serialised to my clients.

## Use case

My service layer provides a `Contact` and a `Address` object, but for the API consumer I want the address to be included into contact that I return to the consumer.  For example

```C#
[Route("contact/{id:int}"]
public Mixin Read(int id)
{
  return new Mixin
     .With(service.GetContact(id))
     .With(service.GetContactAddress(id));
}
```
