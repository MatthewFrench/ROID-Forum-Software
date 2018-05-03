library DatabaseController;

import 'package:mongo_dart/mongo_dart.dart';
import '../Server.dart';
import 'dart:io';
import 'dart:async';
import 'Account.dart';
import 'Post.dart';
import 'Forum.dart';
import 'Section.dart';
import 'Thread.dart';

class DatabaseController {
  Server server;

  Db db;

  DatabaseController(Server s) {
    server = s;
    db = new Db("mongodb://localhost:27017/GMG");
    openDatabase();
    ProcessSignal.SIGHUP.watch().listen((_) { closeDatabase(); });
    ProcessSignal.SIGTERM.watch().listen((_) { closeDatabase(); });
    ProcessSignal.SIGINT.watch().listen((_) { closeDatabase(); });

    /*
    Get section count for forum:
	- Count Search all sections where ForumID = forum

Get thread count:
	- Count Search all threads where SectionID = section, ForumID = forum

Get post count:
	- Count Search all posts where ThreadID = thread, SectionID = section, forumID = forum

How to get order:
	Of posts: - Use Timestamp
	Of threads: - Use last updated timestamp
	Of sections: - Use last updated timestamp
	Of forums: - Use ID

Get Last Post for forum:
	- Search for ForumID and get one, sort by last edited date which should equal last created date at first

Get last 30 posts:
	- Search for 30 threads, sort by last updated date, get the last updated post for each.
     */

    //Collections
    //    IDCount
    //        Name
    //        Count
    //    Accounts
    //        ID
    //        DisplayName
    //        Username
    //        Password
    //        Email
    //        CreatedDate
    //        AvatarURL
    //    Forums
    //        ID
    //        Name
    //        Description
    //    Sections
    //        ID
    //        ForumID
    //        CreatorID
    //        Name
    //        Description
    //        CreatedDate
    //    Threads
    //        ID
    //        SectionID
    //        ForumID
    //        CreatorID
    //        Name
    //        CreatedDate
    //        Views
    //    Posts
    //        ID
    //        ThreadID
    //        SectionID
    //        ForumID
    //        CreatorID
    //        Content
    //        CreatedDate
    //        EditedDate
  }
  Future openDatabase() async {
    bool connected = await db.open();
    if (connected) {
      print('Connected to database');
    } else {print('Did not connect to database');}

    bool authenticated = await db.authenticate('GMGAdmin', 'passwordSuperSecure123');
    if (authenticated) {
      print('Authenticated in GMG database');
    } else {print('Not authenticated in GMG database');}


    if (await getIDCountsCount() == 0) await createIDCounts();

    int forumCount = await getForumCount();
    if (forumCount == 0) {
      //Create Test account
      int accountID = await createAccount('Gan', 'test', 'password', 'fakeemail', '');
      //Create forum
      int forumID = await createForum('Announcements', 'Major Announcements and New Events');
      await createForum('All Talk', 'Talk about anything.');
      int sectionID = await createSection(forumID, accountID, 'Forum News', 'News about the forum!');
      int threadID = await createThread(forumID, sectionID, accountID, 'New forum and website!');
      await createPost(forumID, sectionID, threadID, accountID, 'DO YOU GUYS SEE THIS!?');
    }

    var AccountCollection = db.collection('Accounts');
    server.forumController.registeredPeople = await AccountCollection.count();

    print("Database opened");
  }
  void closeDatabase() {
    db.close().then((_){
      print("Closed database connection");
    });
  }

  Future<String> getThreadName(int threadID) async {
    var ThreadCollection = db.collection('Threads');
    Map m = await ThreadCollection.findOne(where.eq('ID', threadID).fields(['Name']));
    return m['Name'];
  }
  Future<String> getSectionDescription(int forumID, int sectionID) async {
    var SectionCollection = db.collection('Sections');
    Map m = await SectionCollection.findOne(where.eq('ID', sectionID).eq('ForumID', forumID).fields(['Description']));
    return m['Description'];
  }
  Future<String> getSectionName(int forumID, int sectionID) async {
    var SectionCollection = db.collection('Sections');
    Map m = await SectionCollection.findOne(where.eq('ID', sectionID).eq('ForumID', forumID).fields(['Name']));
    return m['Name'];
  }
  Future<String> getForumDescription(int forumID) async {
    var ThreadCollection = db.collection('Forums');
    Map m = await ThreadCollection.findOne(where.eq('ID', forumID).fields(['Description']));
    return m['Description'];
  }
  Future<String> getForumName(int forumID) async {
    var ThreadCollection = db.collection('Forums');
    Map m = await ThreadCollection.findOne(where.eq('ID', forumID).fields(['Name']));
    return m['Name'];
  }

  Future<String> getDiplayNameForAccount(int accountID) async {
    var AccountCollection = db.collection('Accounts');
    Map m = await AccountCollection.findOne(where.eq('ID', accountID).fields(['DisplayName']));
    return m['DisplayName'];
  }
  /*
    Get section count for forum:
	- Count Search all sections where ForumID = forum
*/
  Future<int> getSectionCountInForum(int forumID) async {
    var SectionCollection = db.collection('Sections');
    return await SectionCollection.count(where.eq('ForumID', forumID));
  }

  /*
Get thread count:
	- Count Search all threads where SectionID = section, ForumID = forum

*/
Future<int> getThreadCountInForum(int forumID) async {
  var ThreadCollection = db.collection('Threads');
  return await ThreadCollection.count(where.eq('ForumID',forumID));
}
  Future<int> getThreadCountInSection(int forumID, int sectionID) async {
    var ThreadCollection = db.collection('Threads');
    return await ThreadCollection.count(where.eq('ForumID',forumID).eq('SectionID', sectionID));
  }
 /*
Get post count:
	- Count Search all posts where ThreadID = thread, SectionID = section, forumID = forum
*/
Future<int> getPostCountInForum(int forumID) async {
  var PostCollection = db.collection('Posts');
  return await PostCollection.count(where.eq('ForumID', forumID));
}
  Future<int> getPostCountInSection(int forumID, int sectionID) async {
    var PostCollection = db.collection('Posts');
    return await PostCollection.count(where.eq('ForumID', forumID).eq('SectionID', sectionID));
  }
  Future<int> getReplyCountInThread(int forumID, int sectionID, int threadID) async {
    var PostCollection = db.collection('Posts');
    return await PostCollection.count(where.eq('ForumID', forumID).eq('SectionID', sectionID).eq('ThreadID', threadID));
  }
  /*
How to get order:
	Of posts: - Use Timestamp
	Of threads: - Use last updated timestamp
	Of sections: - Use last updated timestamp
	Of forums: - Use ID
*/
  /*
Get Last Post for forum:
	- Search for ForumID and get one, sort by last edited date which should equal last created date at first
*/
  Future<Post> getLastPostForForum(int forumID) async {
    var PostCollection = db.collection('Posts');
    Map m = await PostCollection.findOne(where.eq('ForumID', forumID).sortBy('EditedDate'));
    if (m == null) {
      return null;
    }
    return new Post(m);
  }
  Future<Post> getLastPostForSection(int forumID, int sectionID) async {
    var PostCollection = db.collection('Posts');
    Map m = await PostCollection.findOne(where.eq('ForumID', forumID).eq('SectionID', sectionID).sortBy('EditedDate'));
    if (m == null) {
      return null;
    }
    return new Post(m);
  }
  Future<Post> getLastPostForThread(int forumID, int sectionID, int threadID) async {
    var PostCollection = db.collection('Posts');
    Map m = await PostCollection.findOne(where.eq('ForumID', forumID).eq('SectionID', sectionID).eq('ThreadID', threadID).sortBy('EditedDate'));
    if (m == null) {
      return null;
    }
    return new Post(m);
  }
  /*
Get last 30 posts:
	- Search for 30 threads, sort by last updated date, get the last updated post for each.
     */
  Future<List<Post>> getLastUpdatedPosts(int count) async {
    var PostCollection = db.collection('Posts');
    List<Post> posts = new List();
    await PostCollection.find(where.sortBy('EditedDate').limit(count)).forEach((Map m){
      posts.add(new Post(m));
    });
    return posts;
  }
  Future<List<Post>> getPosts(int forumID, int sectionID, int threadID) async {
    //    Posts
    //        ID
    //        ThreadID
    //        SectionID
    //        ForumID
    //        CreatorID
    //        Content
    //        CreatedDate
    //        EditedDate
    var PostCollection = db.collection('Posts');
    List<Post> posts = new List();
    await PostCollection.find(where.eq("ForumID", forumID).eq('SectionID', sectionID).eq('ThreadID', threadID)
        .fields(["ID", "ThreadID", "SectionID", "ForumID", "CreatorID", "Content", "CreatedDate", "EditedDate"])).forEach((Map m){
      posts.add(new Post(m));
    });
    return posts;
  }

  Future<List<Thread>> getThreads(int forumID, int sectionID) async {
    //    Threads
    //        ID
    //        SectionID
    //        ForumID
    //        CreatorID
    //        Name
    //        CreatedDate
    //        Views
    var ThreadCollection = db.collection('Threads');
    List<Thread> threads = new List();
    await ThreadCollection.find(where.eq("ForumID", forumID).eq('SectionID', sectionID)
        .fields(["ID", "SectionID", "ForumID", "CreatorID", "Name", "CreatedDate", "Views"])).forEach((Map m){
      threads.add(new Thread(m));
    });
    return threads;
  }

  Future<List<Section>> getSections(int forumID) async {
    //    Sections
    //        ID
    //        ForumID
    //        CreatorID
    //        Name
    //        Description
    //        CreatedDate
    var SectionCollection = db.collection("Sections");
    List<Section> sections = new List();
    await SectionCollection.find(where.eq("ForumID", forumID)
        .fields(["Name", "Description", "ID", "ForumID", "CreatorID", "CreatedDate"])).forEach((Map m){
      sections.add(new Section(m));
    });
    return sections;
  }

  Future<List<Forum>> getForums() async {
    var ForumCollection = db.collection("Forums");
    List<Forum> forums = new List();
    await ForumCollection.find(where.sortBy("ID").fields(["Name", "Description", "ID"])).forEach((Map m){
      forums.add(new Forum(m));
    });
    return forums;
  }
  Future<String> getAvatarForAccount(int accountID) async {
    var AccountCollection = db.collection("Accounts");
    Map m = await AccountCollection.findOne(where.eq("ID", accountID).fields(['AvatarURL']));
    return m['AvatarURL'];
  }
  Future<bool> accountExists(String username, String password) async {
    var AccountCollection = db.collection("Accounts");
    int i = await AccountCollection.count(where.eq("Username", username.toLowerCase()).eq("Password", password));

    return i > 0;
  }

  Future<bool> accountNameExistsOrEmailTaken(String username, String email) async {
    var AccountCollection = db.collection("Accounts");
    int i = await AccountCollection.count(where.eq("Username", username.toLowerCase()).or(where.eq("Email", email.toLowerCase())));

    return i > 0;
  }
  Future<bool> accountNameExists(String username) async {
    var AccountCollection = db.collection("Accounts");
    int i = await AccountCollection.count(where.eq("Username", username.toLowerCase()));

    return i > 0;
  }
  Future<Account> getAccount(String username, String password) async {
    var AccountCollection = db.collection("Accounts");
    Map m = await AccountCollection.findOne(where.eq("Username", username.toLowerCase()).eq("Password", password));
    if (m == null) {
      return null;
    }
    return new Account(m);
  }
  Future<Account> getAccountByID(int id) async {
    var AccountCollection = db.collection("Accounts");
    Map m = await AccountCollection.findOne(where.eq("ID", id));
    if (m == null) {
      return null;
    }
    return new Account(m);
  }

  Future updateAccountAvatarURL(int accountID, String avatarURL) async {
    var AccountCollection = db.collection('Accounts');
    await AccountCollection.update(where.eq('ID', accountID), modify.set('AvatarURL', avatarURL));
  }
  Future<int> getIDCountsCount() async {
    var IDCountCollection = db.collection('IDCount');
    return await IDCountCollection.count();
  }
  Future createIDCounts() async {
    //    IDCount
    //        Name
    //        Count
    var IDCountCollection = db.collection('IDCount');
    await IDCountCollection.insertAll([
      {'Name': 'AccountIDCount', 'Count':0},
      {'Name': 'PostIDCount', 'Count':0},
      {'Name': 'ThreadIDCount', 'Count':0},
      {'Name': 'SectionIDCount', 'Count':0},
      {'Name': 'ForumIDCount', 'Count':0}
    ]);
  }
  Future<int> getAndIncrementIDCount(String name) async {
    //Get the IDCount and increment it by 1
    Map m = await db.executeDbCommand(DbCommand.createQueryDbCommand(db,
        {
          'findAndModify': 'IDCount',
          'query': { 'Name': name },
          'update': { '\$inc': { 'Count': 1 } }
        }
    ));
    return m['value']['Count'];
  }
  Future<int> createAccount(String displayName, String username, String password, String email, String avatarURL) async {
    //    Accounts
    //        ID
    //        DisplayName
    //        Username
    //        Password
    //        Email
    //        CreatedDate
    //        AvatarURL

    //Get the AccountIDCount and increment it by 1
    int accountIDCount = await getAndIncrementIDCount('AccountIDCount');

    //Create post object first
    var AccountCollection = db.collection('Accounts');
    await AccountCollection.insert(
        {
          'ID': accountIDCount,
          'DisplayName': displayName,
          'Username': username.toLowerCase(),
          'Password': password,
          'Email': email.toLowerCase(),
          'AvatarURL': avatarURL,
          'CreatedDate': new DateTime.now().toUtc()
        });

    //Return account ID
    return accountIDCount;
  }
  Future<int> createPost(int inForumID, int inSectionID, int inThreadID, int creatorID, String content) async {
    //    Posts
    //        ID
    //        ThreadID
    //        SectionID
    //        ForumID
    //        CreatorID
    //        Content
    //        CreatedDate
    //        EditedDate

    //Get the PostIDCount and increment it by 1
    int postIDCount = await getAndIncrementIDCount('PostIDCount');

    //Create post object first
    var PostCollection = db.collection('Posts');
    await PostCollection.insert(
    {
      'ID': postIDCount,
      'ForumID': inForumID,
      'SectionID': inSectionID,
      'ThreadID': inThreadID,
      'CreatorID': creatorID,
      'Content': content,
      'CreatedDate': new DateTime.now().toUtc(),
      'EditedDate': new DateTime.now().toUtc()
    });
    //Place post in the correct spot in the thread
    //var ThreadCollection = db.collection('Threads');
    //await ThreadCollection.update(where.eq('ID', inThreadID), modify.push('Posts', postIDCount));
    //Update thread LastUpdatedDate
    //await ThreadCollection.update(where.eq('ID', inThreadID), modify.set('LastUpdatedDate', new DateTime.now()));

    //Return post ID
    return postIDCount;
  }
  Future<int> createThread(int inForumID, int inSectionID, int creatorID, String name) async {
    //    Threads
    //        ID
    //        SectionID
    //        ForumID
    //        CreatorID
    //        Name
    //        CreatedDate
    //        Views

    //Get the ThreadIDCount and increment it by 1
    int threadIDCount = await getAndIncrementIDCount('ThreadIDCount');

    //Create thread object first
    var ThreadCollection = db.collection('Threads');
    await ThreadCollection.insert(
        {
          'ID': threadIDCount,
          'ForumID': inForumID,
          'SectionID': inSectionID,
          'CreatorID': creatorID,
          'Name': name,
          'CreatedDate': new DateTime.now().toUtc(),
          'Views': 0
        });

    //Place thread in the correct spot in the section
    //var SectionCollection = db.collection('Sections');
    //await SectionCollection.update(where.eq('ID', inSectionID), modify.push('Threads', threadIDCount));

    //Return thread ID
    return threadIDCount;
  }
  Future<int> createSection(int inForumID, int creatorID, String name, String description) async {
    //    Sections
    //        ID
    //        ForumID
    //        CreatorID
    //        Name
    //        Description
    //        CreatedDate

    //Get the SectionIDCount and increment it by 1
    int sectionIDCount = await getAndIncrementIDCount('SectionIDCount');

    //Create thread object first
    var SectionCollection = db.collection('Sections');
    await SectionCollection.insert(
        {
          'ID': sectionIDCount,
          'ForumID': inForumID,
          'CreatorID': creatorID,
          'Name': name,
          'Description': description,
          'CreatedDate': new DateTime.now().toUtc()
        });

    //Place section in the correct spot in the forum
    //var ForumCollection = db.collection('Forums');
    //await ForumCollection.update(where.eq('ID', inForumID), modify.push('Sections', sectionIDCount));

    //Return section ID
    return sectionIDCount;
  }
  Future<int> createForum(String name, String description) async {
    //    Forums
    //        ID
    //        Name
    //        Description

    //Get the ForumIDCount and increment it by 1
    int forumIDCount = await getAndIncrementIDCount('ForumIDCount');

    //Create forum object first
    var ForumCollection = db.collection('Forums');
    await ForumCollection.insert(
        {
          'ID': forumIDCount,
          'Name': name,
          'Description': description
        });

    //Return forum ID
    return forumIDCount;
  }
  Future<int> getForumCount() async {
    var ForumCollection = db.collection('Forums');
    return await ForumCollection.count();
  }
}
