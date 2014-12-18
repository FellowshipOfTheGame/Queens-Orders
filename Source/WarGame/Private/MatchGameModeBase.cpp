// Fill out your copyright notice in the Description page of Project Settings.

#include "WarGame.h"
#include "MatchGameModeBase.h"
#include "QueenController.h"
#include "QueenCamera.h"


AMatchGameModeBase::AMatchGameModeBase(const FObjectInitializer& ObjectInitializer) : Super(ObjectInitializer)
{
	//DefaultPawnClass = AQueenCamera::StaticClass();
	PlayerControllerClass = AQueenController::StaticClass();
}