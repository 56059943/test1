/********************************************************************************
** Form generated from reading UI file 'HelloWorld.ui'
**
** Created by: Qt User Interface Compiler version 5.7.1
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_HELLOWORLD_H
#define UI_HELLOWORLD_H

#include <QtCore/QVariant>
#include <QtWidgets/QAction>
#include <QtWidgets/QApplication>
#include <QtWidgets/QButtonGroup>
#include <QtWidgets/QHeaderView>
#include <QtWidgets/QMainWindow>
#include <QtWidgets/QMenuBar>
#include <QtWidgets/QStatusBar>
#include <QtWidgets/QToolBar>
#include <QtWidgets/QWidget>

QT_BEGIN_NAMESPACE

class Ui_HelloWorldClass
{
public:
    QMenuBar *menuBar;
    QToolBar *mainToolBar;
    QWidget *centralWidget;
    QStatusBar *statusBar;

    void setupUi(QMainWindow *HelloWorldClass)
    {
        if (HelloWorldClass->objectName().isEmpty())
            HelloWorldClass->setObjectName(QStringLiteral("HelloWorldClass"));
        HelloWorldClass->resize(600, 400);
        menuBar = new QMenuBar(HelloWorldClass);
        menuBar->setObjectName(QStringLiteral("menuBar"));
        HelloWorldClass->setMenuBar(menuBar);
        mainToolBar = new QToolBar(HelloWorldClass);
        mainToolBar->setObjectName(QStringLiteral("mainToolBar"));
        HelloWorldClass->addToolBar(mainToolBar);
        centralWidget = new QWidget(HelloWorldClass);
        centralWidget->setObjectName(QStringLiteral("centralWidget"));
        HelloWorldClass->setCentralWidget(centralWidget);
        statusBar = new QStatusBar(HelloWorldClass);
        statusBar->setObjectName(QStringLiteral("statusBar"));
        HelloWorldClass->setStatusBar(statusBar);

        retranslateUi(HelloWorldClass);

        QMetaObject::connectSlotsByName(HelloWorldClass);
    } // setupUi

    void retranslateUi(QMainWindow *HelloWorldClass)
    {
        HelloWorldClass->setWindowTitle(QApplication::translate("HelloWorldClass", "HelloWorld", Q_NULLPTR));
    } // retranslateUi

};

namespace Ui {
    class HelloWorldClass: public Ui_HelloWorldClass {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_HELLOWORLD_H
