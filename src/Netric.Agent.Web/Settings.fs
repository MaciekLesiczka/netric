namespace Netric.Agent.Web
    open Netric.Configuration.Profilee
    module Settings = 
        type Site = {name:string;installed:bool}
        type Attributes = 
            {
                assemblies:string;
                sites:list<Site>
            }
        let clrConfigurator = new ClrConfigurator()
        let siteInstaller = new SiteInstaller()            
        let get () = 
            
            let installed = siteInstaller.GetSitesWithInstalledApm() |> Set.ofSeq            
            let sites= siteInstaller.GetAllSites() |> Seq.map (fun x -> {name=x;installed=installed.Contains(x)}) |> List.ofSeq
            {
                assemblies = clrConfigurator.GetProfiledAssemblies();
                sites = sites
            }
        let set form = 
            let result = ("", [])            
            let (assemblies, sites) = form |> List.fold (fun (assemblies,sites) x -> match x with
                                                                                     | ("assemblies",Some(value)) -> (value,sites)
                                                                                     | ("sites", Some(value)) -> (assemblies,value::sites)
                                                                                     | _ -> (assemblies,sites)) result 
            let currentSettings = get()

            if clrConfigurator.GetProfiledAssemblies() <> assemblies then
               clrConfigurator.SetProfiledAssemblies(assemblies)

            let installed = siteInstaller.GetSitesWithInstalledApm() |> Set.ofSeq
            let newSites = sites |> Set.ofSeq
            let toInstall =  sites |> Seq.filter( fun x -> not(installed.Contains(x)) )
            let toUninstall =  installed |> Seq.filter( fun x -> not(newSites.Contains(x)) )

            for s in toInstall do
                siteInstaller.Install(s)

            for s in toUninstall do
                siteInstaller.Uninstall(s)            
            let needIIsreset = 
                if clrConfigurator.GetProfiledAssemblies() <> assemblies then
                   clrConfigurator.SetProfiledAssemblies(assemblies)
                   true
                else
                   false
            get(),needIIsreset

            


            
