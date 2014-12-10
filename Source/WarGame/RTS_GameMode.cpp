// Fill out your copyright notice in the Description page of Project Settings.

#include "WarGame.h"
#include "RTS_GameMode.h"
#include "RTS_Controller.h"
#include "RTS_Camera.h"


ARTS_GameMode::ARTS_GameMode(const FObjectInitializer& ObjectInitializer) : Super(ObjectInitializer)
{
	DefaultPawnClass = ARTS_Camera::StaticClass();
	PlayerControllerClass = ARTS_Controller::StaticClass();
}