using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.engine2;
using Unity.UIWidgets.foundation;
//using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using FontStyle = Unity.UIWidgets.ui.FontStyle;
using Image = Unity.UIWidgets.widgets.Image;
using TextStyle = Unity.UIWidgets.painting.TextStyle;
using ui_ = Unity.UIWidgets.widgets.ui_;
using Unity.UIWidgets.cupertino;
using Unity.UIWidgets.rendering;
//using UIWidgetsGallery.gallery;
using Unity.UIWidgets.service;
using Brightness = Unity.UIWidgets.ui.Brightness;
using UnityEngine;
using System;
using UIWidgetsGallery.gallery;
using Color = Unity.UIWidgets.ui.Color;
using Random = UnityEngine.Random;

namespace UIWidgetsSample
{
    public class TextTest : UIWidgetsPanel
    {
        protected void OnEnable()
        {
            base.OnEnable();
        }

        protected override void main()
        {
            ui_.runApp(new MyApp());
        }

        class MyApp : StatelessWidget
        {
            public override Widget build(BuildContext context)
            {
                return new CupertinoApp(
                    home: new HomeScreen()//new DetailScreen1("ok")
                    //color: Color.white
                );
            }
        }

        class HomeScreen : StatelessWidget
        {
            public override Widget build(BuildContext context)
            {
                
                return new CupertinoNavigationDemo();
               
                //return new CupertinoPickerDemo();
                return new CupertinoTabScaffold(
                        tabBar: new CupertinoTabBar(
                            items: new List<BottomNavigationBarItem>(){
                            new BottomNavigationBarItem(
                                icon: new Icon(CupertinoIcons.book_solid),
                                title: new Text("articles")
                            ),
                            new BottomNavigationBarItem(
                                icon: new Icon(CupertinoIcons.eye_solid),
                                title: new Text("views")
                            )
                        }

                    ),
                    tabBuilder: (context1, i)=>{
                        return new CupertinoTabView(
                            builder: (context2)=>{
                            return new CupertinoPageScaffold(
                                navigationBar: new CupertinoNavigationBar(
                                    middle:(i==0) ? new Text("articles") : new Text("views")
                                ),
                                child: new Center(
                                    child: new Text($"this is tab #{i}",
                                        style: CupertinoTheme.of(context)
                                            .textTheme
                                            .navActionTextStyle
    
                                    )
                                )
                            );
                        }
                        );
                    }
                    );
                
            }
        }

        public class DetailScreen1 : StatelessWidget
            {
                public DetailScreen1(string topic)
                {
                    this.topic = topic;
                }

                public string topic;

                public override Widget build(BuildContext context)
                {
                    return new CupertinoPageScaffold(
                        //backgroundColor: Color.white,
                        child: new Center(
                            child: new Text(
                                "hello world"
                                 //style : new TextStyle(color: CupertinoColors.activeBlue)
                                //style : new TextStyle(color: Color.white)
                            )
                        )
                    );
                }
            }
        }
    }
