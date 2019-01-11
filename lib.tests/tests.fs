namespace Demo.Types.Lib.Tests

open Microsoft.Extensions.Logging

open Xunit
open Xunit.Abstractions

open Example.Serialisation

open Demo.Types

type LibShould( oh :ITestOutputHelper ) =
    
    let logger =
        let options = { Logging.Options.Default with OutputHelper = Some oh }
        Logging.CreateLogger options
        
    [<Theory>]
    [<InlineData("json")>]
    [<InlineData("binary")>]
    member this.``RoundTripTheDemoType`` (contentType:string) =
        
        let serde =
            Serde.Make( SerdeOptions.Default )
            
        let nTypeSerdesRegistered =
            serde.TryRegisterAssembly typeof<Address>.Assembly 
        
        logger.LogInformation( "Registered {nSerialisersRegistered} type serdes", nTypeSerdesRegistered )
        
        let example =
            Address.Make( 1, "High Street" )

        let bytes =
            example |> Helpers.Serialise serde contentType
            
        if contentType = "json" then
            logger.LogInformation( "{JSON}", bytes |> System.Text.Encoding.UTF8.GetString )
        else
            logger.LogInformation( "{Bytes}", bytes )
            
        let example' =
            bytes |> Helpers.DeserialiseT<_> serde contentType
            
        Assert.Equal( example', example )