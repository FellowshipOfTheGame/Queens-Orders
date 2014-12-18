// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/GameMode.h"
#include "MatchGameModeBase.generated.h"

/**
 * 
 */
UCLASS()
class WARGAME_API AMatchGameModeBase : public AGameMode
{
	GENERATED_BODY()

public:	
	AMatchGameModeBase(const FObjectInitializer& ObjectInitializer);
	
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Buildings")
	UClass *buildingBarracks;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Buildings")
	UClass *buildingRanger;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Buildings")
	UClass *buildingTower;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Buildings")
	UClass *buildingCitadel;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Buildings")
	UClass *buildingGate;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Buildings")
	UClass *buildingKeep;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Buildings")
	UClass *buildingStoneMine;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Buildings")
	UClass *buildingGoldMine;
};
