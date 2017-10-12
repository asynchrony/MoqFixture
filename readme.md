# MoqFixture

`MoqFixture` exists to simplify the setup of mocks for Moq-driven unit testing of classes.

## Usage

`MoqFixture` creates a `TestObject` for your fixture, and provides a `Mock<TMock>()` function you can use to retrieve Moq mocks for your TestObject's dependencies.

Simply have your fixture inherit from `MoqFixture<TypeUnderTest>`, and access the `Mock<TMock>()` / `TestObject` of the base class.

## Example

### Test:
```C#
[TestFixture]
//Tell MoqFixture the type you're mocking dependencies for
public class UserInfoControllerFixture : MoqFixture<UserInfoController> 
{
    [Test]
    public void ShouldReturnFoundUserInfo()
    {
        var expectedUser = new User
        {
            Username = "tom",
            FullName = "tom thumb"
        };

	//The magic bit
        Mock<IUserService>() 
		.Setup(x => x.ResolveCurrentUser())
		.Returns(expectedUser);

	//TestObject is provided within the Fixture
        UserInfoView result = TestObject.Get();

        Assert.That(result.UserName, Is.EqualTo(expectedUser.Username));
        Assert.That(result.FullName, Is.EqualTo(expectedUser.FullName));
    }
}
```

### Implementation:
```C#
public class UserInfoController : ApiController
{
    private readonly IUserService _userService;

    //Automatically called and injected by MoqFixture
    public UserInfoController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("api/v1/userinfo")]
    public UserInfoView Get()
    {
        var curUser = _userService.ResolveCurrentUser();
        return new UserInfoView
        {
            FullName = curUser.FullName,
            UserName = curUser.Username
        };
    }
}
```

## Limitations

`MoqFixture` cannot mock dependencies where there is ambiguity. This includes:

* Classes with multiple constructors
* Classes with multiple constructor arguments of the same type

`MoqFixture` also inherits the limitations of Moq itself, namely:

* It cannot mock dependencies that are sealed classes
* It cannot mock dependencies that are not classes or interfaces
* It cannot mock members that are not virtual
