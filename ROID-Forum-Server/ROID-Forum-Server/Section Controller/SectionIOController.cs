using System;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class SectionIOController
    {
        SectionController controller;
        public SectionIOController(SectionController c)
        {
            controller = c;
        }
        public Dictionary<string, object> getDataMap()
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["threadIDs"] = controller.threadController.threadIDs;
            List<Dictionary<string, object>> threads = new List<Dictionary<string, object>>();
            for (int i = 0; i < controller.threadController.threads.Count; i++)
            {
                ThreadInfo t = controller.threadController.threads[i];
                threads.Add(t.toMap());
            }
            m["threads"] = threads;
            return m;
        }
        public void processDataMap(Dictionary<string, object> m)
        {
            int threadIDs = (int)m["threadIDs"];
            controller.threadController.threadIDs = threadIDs;
            List<Dictionary<string, object>> threadList = (List<Dictionary<string, object>>)m["threads"];
            for (int i = 0; i < threadList.Count; i++)
            {
                Dictionary<string, object> threadMap = threadList[i];
                controller.threadController.threads.Add(new ThreadInfo(threadMap));
            }
        }
        public void saveAllData()
        {
            /*
              try {
                File file = new File("bin/Saved Data/codingcontroller.json");
                file.exists().then((bool e) {
                  if (e) {
                    file.delete().then((var f) {
                      var output = file.openWrite();
                      output.write(JSON.encode(getDataMap()));
                      output.close();
                    });
                  } else {
                    var output = file.openWrite();
                    output.write(JSON.encode(getDataMap()));
                    output.close();
                  }
                });
              } catch (exception, stackTrace) {
                print(exception);
                print(stackTrace);
              }
              */
        }
        public void loadAllData()
        {
            /*
              try {
                File file = new File("bin/Saved Data/codingcontroller.json");
                file.exists().then((bool e) {
                  if (e) {
                    Future<String> finishedReading = file.readAsString();
                    finishedReading.then((text) {
                      processDataMap(JSON.decode(text));
                      print("Loaded coding controller");
                    });
                  }
                });
              } catch (exception, stackTrace) {
                print(exception);
                print(stackTrace);
              }
              */
        }
    }
}
