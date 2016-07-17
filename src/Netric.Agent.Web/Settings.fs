namespace Netric.Agent.Web
    open Netric.Configuration.Profilee
    
    module Settings = 
        type Site = {name:string;installed:bool}
        type Info = 
            {
                assemblies:string;
                sites:Site[]
            }

        let clrConfigurator = new ClrConfigurator()
        let siteInstaller = new SiteInstaller()            
        let get () =                         
            {
                assemblies = clrConfigurator.GetProfiledAssemblies();
                sites = 
                    let installed = siteInstaller.GetSitesWithInstalledApm() |> Set.ofSeq
                    siteInstaller.GetAllSites() |> Seq.map (fun x -> {name=x;installed=installed.Contains(x)}) |> Array.ofSeq
            }        
        let set newSettings =             
                        
            for s in newSettings.sites do
                if s.installed then
                    siteInstaller.Install(s.name)
                else 
                    siteInstaller.Uninstall(s.name)

            clrConfigurator.SetProfiledAssemblies(newSettings.assemblies)

            


            
